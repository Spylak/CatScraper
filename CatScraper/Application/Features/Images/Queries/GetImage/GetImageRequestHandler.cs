using CatScraper.Application.Abstractions;
using CatScraper.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CatScraper.Application.Features.Images.Queries.GetImage;

public class GetImageRequestHandler : IRequestHandler<GetImageRequest, ErrorOr<GetImageResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private const string CacheKeyPrefix = "ImageCache_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    public GetImageRequestHandler(IAppDbContext dbContext, IMemoryCache memoryCache)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
    }

    public async Task<ErrorOr<GetImageResponse>> Handle(GetImageRequest request, CancellationToken cancellationToken)
    {
        string cacheKey = $"{CacheKeyPrefix}{request.Id}";

        if (_memoryCache.TryGetValue(cacheKey, out GetImageResponse? cachedImage))
        {
            if (cachedImage is not null)
            {
                return cachedImage;
            }
        }
        
        var image = await _dbContext
            .Set<Cat>()
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new GetImageResponse
            {
                Image = c.Image
            })
            .FirstOrDefaultAsync(cancellationToken);
    
        if (image is null)
            return Error.NotFound(description: "Image not found");

        _memoryCache.Set(cacheKey, image, CacheDuration);
        
        return image;
    }
}