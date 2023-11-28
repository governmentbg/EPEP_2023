using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace eCase.Data.Linq
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WithOffsetAndLimit<T>(this IQueryable<T> query, int offset, int? limit)
        {
            if (offset > 0)
            {
                query = query.Skip(offset);
            }

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            return query;
        }

        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, Expression<Func<T, object>>[] includes)
        {
            var includeQuery = query;

            foreach(var include in includes)
            {
                includeQuery = includeQuery.Include(include);
            }

            return includeQuery;
        }
    }
}
