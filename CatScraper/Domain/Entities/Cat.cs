using CatScraper.Domain.Common;
using CatScraper.Domain.ValueObjects;

namespace CatScraper.Domain.Entities;

public class Cat : BaseEntity, ICreatedAtUtc
{
    public required string CatId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public required Image Image { get; set; }
    public ICollection<CatTag> CatTags { get; set; } = [];
    public DateTimeOffset Created { get; }
}