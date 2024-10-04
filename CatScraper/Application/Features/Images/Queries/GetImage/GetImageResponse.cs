using CatScraper.Domain.ValueObjects;

namespace CatScraper.Application.Features.Images.Queries.GetImage;

public class GetImageResponse
{
    public required Image Image { get; set; }
}