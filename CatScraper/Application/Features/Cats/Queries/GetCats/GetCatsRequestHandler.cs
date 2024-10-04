using CatScraper.Application.Abstractions;
using CatScraper.Domain.Entities;
using CatScraper.Domain.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatScraper.Application.Features.Cats.Queries.GetCats;

public class GetCatsRequestHandler : IRequestHandler<GetCatsRequest, ErrorOr<List<GetCatsResponse>>>
{
    private readonly IAppDbContext _dbContext;

    public GetCatsRequestHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<GetCatsResponse>>> Handle(GetCatsRequest request, CancellationToken cancellationToken)
    {
        var cats = await _dbContext.Set<Cat>()
            .WhereIf(request.Tag is not null, i =>
                i.CatTags
                    .Where(ct => ct.Tag != null)
                    .Any(j => j.Tag!.Name == request.Tag))
            .SkipIf(request is { Page: not null, PageSize: not null }, (request.Page - 1) * request.PageSize)
            .TakeIf(request.PageSize.HasValue, request.PageSize)
            .Select(c => new GetCatsResponse
            {
                Id = c.Id,
                CatId =c.CatId,
                Height = c.Height,
                Width = c.Width,
                Created = c.Created,
                Tags = c.CatTags
                    .Where(ct => ct.Tag != null)
                    .Select(ct => ct.Tag!.Name)
                    .Distinct()
                    .ToList(),
                Url = $"{request.BaseUrl}/api/images/get/{c.Id}"
            })
            .ToListAsync(cancellationToken);
        
        return cats;
    }
}