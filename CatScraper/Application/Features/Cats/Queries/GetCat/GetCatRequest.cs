using MediatR;

namespace CatScraper.Application.Features.Cats.Queries.GetCat;

public class GetCatRequest : IRequest<ErrorOr<GetCatResponse>>
{
    public required int Id { get; set; }
    public required string BaseUrl { get; set; }
}