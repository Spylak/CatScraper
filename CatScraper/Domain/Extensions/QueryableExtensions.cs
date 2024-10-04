using System.Linq.Expressions;

namespace CatScraper.Domain.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        if (condition)
        {
            return source.Where(predicate);
        }

        return source;
    }
    
    public static IQueryable<T> TakeIf<T>(
        this IQueryable<T> source,
        bool condition,
        int? takeCount)
    {
        if (condition && takeCount.HasValue)
        {
            return source.Take(takeCount.Value);
        }

        return source;
    }
    
    public static IQueryable<T> SkipIf<T>(
        this IQueryable<T> source,
        bool condition,
        int? skipCount)
    {
        if (condition && skipCount.HasValue)
        {
            return source.Skip(skipCount.Value);
        }

        return source;
    }
}