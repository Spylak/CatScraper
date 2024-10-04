using CatScraper.Domain.Common;
using CatScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CatScraper.Application.Abstractions;

public interface IAppDbContext
{
    DatabaseFacade Database { get; }
    DbSet<T> Set<T>() where T : DbEntity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}