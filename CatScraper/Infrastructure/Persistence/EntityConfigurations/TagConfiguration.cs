using CatScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatScraper.Infrastructure.Persistence.EntityConfigurations;

public class TagConfiguration : BaseEntityConfiguration<Tag>
{
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        base.Configure(builder);

        builder.ToTable(name: "Tags");

        builder.HasIndex(i => i.Name)
            .IsUnique();
        
        builder.Property(i => i.Created)
            .HasDefaultValueSql("getutcdate()");
        
        builder.HasMany(c => c.CatTags)
            .WithOne(i => i.Tag)
            .HasForeignKey(i => i.TagId);
    }
}