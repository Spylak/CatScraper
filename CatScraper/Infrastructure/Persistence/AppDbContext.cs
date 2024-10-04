using CatScraper.Application.Abstractions;
using CatScraper.Domain.Common;
using CatScraper.Domain.Entities;
using CatScraper.Infrastructure.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace CatScraper.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurations();
    }

    public new DbSet<T> Set<T>() where T : DbEntity
    {
        return base.Set<T>();
    }
}