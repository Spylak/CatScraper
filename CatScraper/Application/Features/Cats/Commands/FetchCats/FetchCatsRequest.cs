using MediatR;

namespace CatScraper.Application.Features.Cats.Commands.FetchCats;

public class FetchCatsRequest : IRequest<ErrorOr<List<FetchCatsResponse>>>
{
    public required string BaseUrl { get; set; }
    public string? ApiKey { get; set; }
}