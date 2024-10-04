using CatScraper.Domain.Entities;
using CatScraper.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatScraper.Infrastructure.Persistence.EntityConfigurations;

public class GlobalCounterConfiguration : BaseEntityConfiguration<GlobalCounter>
{
    public override void Configure(EntityTypeBuilder<GlobalCounter> builder)
    {
        base.Configure(builder);

        builder.ToTable("GlobalCounters");

        builder.HasData(new GlobalCounter()
        {
            Id = 1,
            Type = GlobalCounterType.CatApiPageSize,
            Value = 0
        });
    }
}