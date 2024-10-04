using CatScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatScraper.Infrastructure.Persistence.EntityConfigurations;

public class CatConfiguration : BaseEntityConfiguration<Cat>
{
    public override void Configure(EntityTypeBuilder<Cat> builder)
    {
        base.Configure(builder);
        
        builder.ToTable(name: "Cats");

        builder.HasIndex(i => i.CatId)
            .IsUnique();
        
        builder.Property(i => i.Created)
            .HasDefaultValueSql("getutcdate()");

        builder.HasMany(c => c.CatTags)
            .WithOne(i => i.Cat)
            .HasForeignKey(i => i.CatId);
        
        builder.OwnsOne(s => s.Image, 
            b =>
            {
                b.ToJson();
            });
    }
}