using CatScraper.Application.Abstractions;
using CatScraper.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatScraper.Application.Features.Images.Queries.GetImage;

public class GetImageRequestHandler : IRequestHandler<GetImageRequest, ErrorOr<GetImageResponse>>
{
    private readonly IAppDbContext _dbContext;

    public GetImageRequestHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<GetImageResponse>> Handle(GetImageRequest request, CancellationToken cancellationToken)
    {
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

        return image;
    }
}