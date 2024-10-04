using CatScraper.Application.Abstractions;
using CatScraper.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatScraper.Application.Features.Cats.Queries.GetCat;

public class GetCatRequestHandler : IRequestHandler<GetCatRequest, ErrorOr<GetCatResponse>>
{
    private readonly IAppDbContext _dbContext;

    public GetCatRequestHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<GetCatResponse>> Handle(GetCatRequest request, CancellationToken cancellationToken)
    {
        var cat = await _dbContext.Set<Cat>()
            .Where(i => i.Id == request.Id)
            .Select(c => new
            {
                c.Id,
                c.CatId,
                c.Height,
                c.Width,
                c.Created,
                Tags = c.CatTags
                    .Where(ct => ct.Tag != null)
                    .Select(ct => ct.Tag!.Name)
                    .Distinct()
                    .ToList()
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (cat is null)
            return Error.NotFound(description: "Cat not found");

        return new GetCatResponse
        {
            Id = cat.Id,
            CatId = cat.CatId,
            Height = cat.Height,
            Width = cat.Width,
            Url = $"{request.BaseUrl}/api/images/get/{cat.Id}",
            Created = cat.Created,
            Tags = cat.Tags
        };
    }
}