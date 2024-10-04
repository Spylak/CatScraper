namespace CatScraper.Application.Dtos.Cats;

public class CatDto
{
    public int Id { get; }
    public DateTimeOffset Created { get; set; }
    public required string CatId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public required string ImageUrl { get; set; }
    public List<string> Tags { get; set; } = [];
}