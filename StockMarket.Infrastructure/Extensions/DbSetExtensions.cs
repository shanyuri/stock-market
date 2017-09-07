using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace StockMarket.Infrastructure.Extensions
{
    public static class DbSetExtensions
    {
        // this beautiful extension method comes from
        // https://stackoverflow.com/a/31162909
        public static T AddIfNotExists<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>> predicate = null)
            where T : class, new()
        {
            var exists = predicate != null ? dbSet.Any(predicate) : dbSet.Any();
            return !exists ? dbSet.Add(entity) : null;
        }
    }
}