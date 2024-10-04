using CatScraper.Domain.Common;

namespace CatScraper.Domain.Entities;

public class CatTag : DbEntity
{
    public int CatId { get; set; }
    public Cat? Cat { get; set; }
    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}