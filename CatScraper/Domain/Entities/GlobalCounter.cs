using CatScraper.Domain.Enums;

namespace CatScraper.Domain.Entities;

public class GlobalCounter : BaseEntity
{
    public required GlobalCounterType Type { get; set; }
    public required int Value { get; set; }
}