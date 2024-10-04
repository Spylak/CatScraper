using CatScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatScraper.Infrastructure.Persistence.EntityConfigurations;

public class CatTagConfiguration : IEntityTypeConfiguration<CatTag>
{
    public void Configure(EntityTypeBuilder<CatTag> builder)
    {
        builder.ToTable(name: "CatTag");
        builder.HasKey(tq => new { tq.CatId, tq.TagId });

        builder.Property(tq => tq.CatId).IsRequired();
        builder.Property(tq => tq.TagId).IsRequired();
        
        builder.HasOne(fs => fs.Cat)
            .WithMany(f => f.CatTags)
            .HasForeignKey(fs => fs.CatId);

        builder.HasOne(fs => fs.Tag)
            .WithMany(s => s.CatTags)
            .HasForeignKey(fs => fs.TagId);
    }
}