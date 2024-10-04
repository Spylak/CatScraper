using MediatR;

namespace CatScraper.Application.Features.Images.Queries.GetImage;

public class GetImageRequest : IRequest<ErrorOr<GetImageResponse>>
{
    public required int Id { get; set; }
}