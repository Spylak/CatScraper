using System.Collections.Concurrent;
using System.Text;
using CatScraper.Application.Abstractions;
using CatScraper.Application.Helpers;
using CatScraper.Domain.Entities;
using CatScraper.Domain.Enums;
using CatScraper.Domain.Extensions;
using CatScraper.Domain.Options;
using CatScraper.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CatScraper.Application.Features.Cats.Commands.FetchCats;

public class FetchCatsRequestHandler : IRequestHandler<FetchCatsRequest, ErrorOr<List<FetchCatsResponse>>>
{
    private string? _catsApiKey;
    private readonly IAppDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CatFetchLockHelper _catFetchLockHelper;
    private ConcurrentDictionary<string, Image> Images { get; set; } = [];

    public FetchCatsRequestHandler(IOptions<CatsApiOption> catsApiOptions, IAppDbContext dbContext,
        IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _catFetchLockHelper = CatFetchLockHelper.Instance;
        _catsApiKey = string.IsNullOrWhiteSpace(catsApiOptions.Value.ApiKey) ? null : catsApiOptions.Value.ApiKey;
    }

    public async Task<ErrorOr<List<FetchCatsResponse>>> Handle(FetchCatsRequest request,
        CancellationToken cancellationToken)
    {
        ErrorOr<List<FetchCatsResponse>> result =
            Error.Failure(description: "FetchCatsRequestHandler is already running. Please wait.");

        await _catFetchLockHelper.AddTaskToQueue(async () =>
        {
            result = await ExecuteHandle(request, cancellationToken);
        });

        return result;
    }

    private async Task<ErrorOr<List<FetchCatsResponse>>> ExecuteHandle(FetchCatsRequest request,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        var fetchCatsResponses = new List<CatsApiResponse>();
        try
        {
            _catsApiKey ??= request.ApiKey;
            if (string.IsNullOrWhiteSpace(_catsApiKey))
            {
                return Error.Unauthorized(description: "Api key not found.");
            }

            var globalCounter = await _dbContext
                .Set<GlobalCounter>()
                .FirstAsync(i => i.Type == GlobalCounterType.CatApiPageSize, cancellationToken);

            var page = globalCounter.Value;
            fetchCatsResponses = await GetFetchCatsResponses(page, cancellationToken);
            if (fetchCatsResponses.Count == 0)
            {
                return Error.Failure(description: "Couldn't retrieve cat images");
            }

            var responses = fetchCatsResponses;
            var existingCatIds = await _dbContext.Set<Cat>()
                .Where(c => responses.Select(j => j.Id).Contains(c.CatId))
                .Select(j => j.CatId)
                .ToListAsync(cancellationToken);

            if (existingCatIds.Count == 25)
                return Error.Failure(description: "Cats are already in our database");

            fetchCatsResponses = fetchCatsResponses
                .Where(i => !existingCatIds.Contains(i.Id))
                .ToList();

            var getImageDataTasks = fetchCatsResponses
                .Select(apiResponse => DownloadImageAsync(apiResponse.Id, apiResponse.Url))
                .ToList();

            await Task.WhenAll(getImageDataTasks);

            var tags = fetchCatsResponses
                .SelectMany(i => i.Breeds.Select(j => j.Temperament))
                .SelectMany(i => i.Split(", "))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(name => new Tag { Name = name.ToPascalCase() })
                .ToList();

            var tagNames = tags
                .Select(j => j.Name)
                .ToList();

            var existingTags = await _dbContext.Set<Tag>()
                .Where(tag => tagNames.Contains(tag.Name))
                .ToListAsync(cancellationToken);

            var newTags = tags
                .Where(i => existingTags.All(j => j.Name != i.Name))
                .ToList();

            if (newTags.Count != 0)
            {
                await _dbContext.Set<Tag>().AddRangeAsync(newTags, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                existingTags.AddRange(newTags);
            }

            var catTagList = PopulateCatTags(fetchCatsResponses, existingTags);

            var catsWithoutBreeds = catTagList
                .Where(i => i.Tag == null && i.Cat != null)
                .Select(i => i.Cat)
                .ToList();

            var catsWithBreeds = catTagList
                .Where(i => i is { Tag: not null, Cat: not null })
                .ToList();

            if (catsWithBreeds.Count != 0)
            {
                await _dbContext.Set<CatTag>()
                    .AddRangeAsync(catsWithBreeds, cancellationToken);
            }

            if (catsWithoutBreeds.Count != 0)
            {
                await _dbContext.Set<Cat>()
                    .AddRangeAsync(catsWithoutBreeds!, cancellationToken);
            }

            globalCounter.Value += 1;
            _dbContext.Set<GlobalCounter>().Update(globalCounter);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return fetchCatsResponses.Select(i => new FetchCatsResponse
                { Url = $"{request.BaseUrl}/api/images/get/{i.Id}" }).ToList();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            if (fetchCatsResponses.Count != 0)
                return fetchCatsResponses.Select(i => new FetchCatsResponse
                    { Url = $"{request.BaseUrl}/api/images/get/{i.Id}" }).ToList();

            return Error.Unexpected(description: ex.InnerException?.Message ?? ex.Message);
        }
    }

    // Local 404 image if Cat Api doesn't provide an image for ease of usage.
    private async Task DownloadImageAsync(string imageId, string imageUrl)
    {
        string imageExtension;
        var client = _httpClientFactory.CreateClient();

        var response = await client.GetAsync(imageUrl);
        response.EnsureSuccessStatusCode();
        var imageData = await response.Content.ReadAsByteArrayAsync();

        var contentType = response.Content.Headers.ContentType?.MediaType;
        if (contentType != null)
        {
            imageExtension = contentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/bmp" => ".bmp",
                _ => ".jpg"
            };
        }
        else
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Content/404.png");
            imageData = await File.ReadAllBytesAsync(filePath);
            imageExtension = ".png";
        }

        Images[imageId] = new Image()
        {
            ImageData = imageData,
            ImageExtension = imageExtension
        };
    }

    private async Task<List<CatsApiResponse>> GetFetchCatsResponses(int page,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("x-api-key", _catsApiKey);
        var urlBldr = new StringBuilder();
        urlBldr.Append("https://api.thecatapi.com/v1/images/search?");
        urlBldr.Append("limit=25");
        urlBldr.Append("&");
        urlBldr.Append("has_breeds=1");
        urlBldr.Append("order=ASC");
        urlBldr.Append("page=");
        urlBldr.Append(page);

        var fetchCatsResponses = await client.GetFromJsonAsync<List<CatsApiResponse>>(urlBldr.ToString(),
            cancellationToken: cancellationToken);

        return fetchCatsResponses ?? [];
    }

    private List<CatTag> PopulateCatTags(List<CatsApiResponse> apiResponses, List<Tag> tags)
    {
        var catTagList = new List<CatTag>();
        foreach (var apiResponse in apiResponses)
        {
            var cat = new Cat()
            {
                CatId = apiResponse.Id,
                Height = apiResponse.Height,
                Width = apiResponse.Width,
                Image = Images[apiResponse.Id]
            };
            catTagList.AddRange(tags
                .Where(i => apiResponse.Breeds
                        .Any(j => j.Temperament.Contains(i.Name)))
                .Select(tag => new CatTag { Cat = cat, Tag = tag }));
        }

        return catTagList;
    }

    private class CatsApiResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Breed> Breeds { get; set; } = [];

        public class Breed
        {
            public string Temperament { get; set; } = string.Empty;
        }
    }
}