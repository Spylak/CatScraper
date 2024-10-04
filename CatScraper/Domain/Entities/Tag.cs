
using CatScraper.Domain.Common;

namespace CatScraper.Domain.Entities;

public class Tag : BaseEntity, ICreatedAtUtc
{
    public required string Name { get; set; }
    public ICollection<CatTag> CatTags { get; set; } = [];
    public DateTimeOffset Created { get; }
}