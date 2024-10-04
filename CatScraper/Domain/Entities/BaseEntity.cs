using CatScraper.Domain.Common;

namespace CatScraper.Domain.Entities;

public class BaseEntity : DbEntity
{
    public int Id { get; init; }
}