using MediatR;

namespace CatScraper.Application.Features.Cats.Queries.GetCats;

public class GetCatsRequest : IRequest<ErrorOr<List<GetCatsResponse>>>
{
    public required string BaseUrl { get; set; }
    public int? Page { get; set; } 
    public int? PageSize { get; set; } 
    public string? Tag { get; set; } 
}