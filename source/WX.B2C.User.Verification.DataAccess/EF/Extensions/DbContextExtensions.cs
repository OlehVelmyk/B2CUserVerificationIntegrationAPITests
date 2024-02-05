using System;
using System.Linq;
using System.Linq.Expressions;
using Community.OData.Linq;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.DataAccess.EF.Extensions
{
    internal static class DbContextExtensions
    {
        public static IQueryable<T> QueryAsNoTracking<T>(this DbContext dbContext, Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            var query = dbContext.Set<T>().AsNoTracking();
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        public static IQueryable<T> Query<T>(this DbContext dbContext, Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            var query = dbContext.Set<T>().AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        public static (IQueryable<TEntity> TotalCountQuery, IQueryable<TEntity> ItemsQuery) ApplyOData<TEntity>(
            this IQueryable<TEntity> query,
            ODataQueryContext oDataContext)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (oDataContext == null)
                throw new ArgumentNullException(nameof(oDataContext));

            var oDataQuery = query.OData();
            if (oDataContext.Filter != null)
                oDataQuery = oDataQuery.Filter(oDataContext.Filter);

            var fullQuery = oDataQuery;
            if (oDataContext.Top != null || oDataContext.Skip != null)
                fullQuery = fullQuery.TopSkip(oDataContext.Top, oDataContext.Skip);
            if (oDataContext.OrderBy != null)
                fullQuery = fullQuery.OrderBy(oDataContext.OrderBy);

            return (oDataQuery.ToOriginalQuery(), fullQuery.ToOriginalQuery());
        }
    }
}
