namespace CatScraper.Domain.ValueObjects;

public class Image
{
    public required byte[] ImageData { get; set; }
    public required string ImageExtension { get; set; }
}