using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Execution;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Extensions
{
    internal static class SqlKataExtensions
    {
        public static Query AsUpdate(this Query query, string column, object value) =>
            query.AsUpdate(new[] { KeyValuePair.Create(column, value) });

        public static Query Where<TEnum>(this Query query, string column, TEnum @enum) where TEnum : struct, Enum =>
            query.Where(column, @enum.ToString());

        public static Join WhereEnums<TEnum>(this BaseQuery<Join> query, string column, IEnumerable<TEnum> enums)
            where TEnum : struct, Enum =>
            query.WhereIn(column, enums.Select(@enum => @enum.ToString()));

        public static Query WhereInEnums<TEnum>(this Query query, string column, IEnumerable<TEnum> enums)
            where TEnum : struct, Enum =>
            query.WhereIn(column, enums.Select(@enum => @enum.ToString()));

        public static Query WhereUserIdIn(this Query query, IEnumerable<Guid> values) =>
            query.WhereInIfNotEmpty("UserId", values);

        public static Query WhereInIfNotEmpty<T>(this Query query, string column, IEnumerable<T> values) =>
            values.IsNullOrEmpty() ? query : query.WhereIn(column, values);


        public static Query Select<T>(this Query query) =>
            query.Select(typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty)
                                  .Select(info => info.Name)
                                  .ToArray());
        
        public static async IAsyncEnumerable<ICollection<T>> PaginateAsync<T>(this IQueryFactory queryFactory, Func<QueryFactory, Query> queryBuilder,BatchJobSettings settings, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var factory = queryFactory.Create();

            var query = queryBuilder(factory);
            var pagination = await query.PaginateAsync<T>(1, settings.ReadingBatchSize, cancellationToken: cancellationToken);
            yield return pagination.List.ToArray();
            while (pagination.HasNext)
            {
                pagination = await pagination.NextAsync(cancellationToken: cancellationToken);
                yield return pagination.List.ToArray();
            }
        }

        public static async Task<int> CountAsync(this IQueryFactory queryFactory, Func<QueryFactory, Query> queryBuilder, CancellationToken cancellationToken)
        {
            using var factory = queryFactory.Create();

            var query = queryBuilder(factory);
            var count = await query.CountAsync<int>(cancellationToken: cancellationToken);
            return count;
        }
    }
}