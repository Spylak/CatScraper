namespace CatScraper.Application.Features.Cats.Queries.GetCats;

public class GetCatsResponse
{
    public required int Id { get; set; }
    public required string CatId { get; set; }
    public required string Url { get; set; }
    public required int Width { get; set; }
    public required int Height { get; set; }
    public required DateTimeOffset Created { get; set; }
    public List<string> Tags { get; set; }
}