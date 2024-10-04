using Microsoft.EntityFrameworkCore;

namespace CatScraper.Infrastructure.Persistence.EntityConfigurations;

public static class EntityConfigurationsExtension
{
    public static void ApplyConfigurations(this ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CatConfiguration());
        builder.ApplyConfiguration(new TagConfiguration());
        builder.ApplyConfiguration(new CatTagConfiguration());
        builder.ApplyConfiguration(new GlobalCounterConfiguration());
    }
}