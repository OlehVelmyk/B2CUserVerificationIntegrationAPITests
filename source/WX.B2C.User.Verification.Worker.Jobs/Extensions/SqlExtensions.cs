using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Worker.Jobs.Extensions
{

    /// <summary>
    /// TODO it seems that should be deleted in phase 2
    /// </summary>
    internal static class SqlExtensions
    {
        /// <summary>
        /// Returns proper SQL where part like: WHERE UserId in ('F2522862-1257-4D80-8F06-90102D899987')
        /// If collection is empty returns empty string.
        /// </summary>
        public static string WhereUserIdIn(this IEnumerable<Guid> users) =>
            WhereIn(users, "UserId");

        /// <summary>
        /// Returns proper SQL where part like: WHERE UserId in ('F2522862-1257-4D80-8F06-90102D899987')
        /// If collection is empty returns empty string.
        /// </summary>
        public static string WhereIn(this IEnumerable<Guid> ids, string columnName) =>
            ids.ColumnIn($"WHERE {columnName}");

        /// <summary>
        /// Returns proper SQL query part for IN operator if collection is not empty.
        /// Otherwise returns empty string.
        /// </summary>
        /// <returns>String like "AND UserId IN ('57e01cad-c689-4257-88c3-9c2ef6b98982', '99994c8f-b81a-4f9c-9291-a96170ed35cf')"</returns>
        public static string AndIn(this IEnumerable<Guid> ids, string columnName) =>
            ids.ColumnIn($"AND {columnName}");

        /// <summary>
        /// Returns proper SQL query part for IN operator if collection is not empty like: "UserId IN ('57e01cad-c689-4257-88c3-9c2ef6b98982', '99994c8f-b81a-4f9c-9291-a96170ed35cf')"
        /// Otherwise returns empty string.
        /// </summary>
        public static string ColumnIn(this IEnumerable<Guid> ids, string columnName)
            => ids.ColumnIn(columnName, "IN");

        /// <summary>
        /// Returns proper SQL query part for IN operator if collection is not empty like: "Word IN ('one', 'two')"
        /// Otherwise returns empty string.
        /// </summary>
        public static string ColumnIn(this IEnumerable<string> items, string columnName)
            => items.ColumnIn(columnName, "IN");

        /// <summary>
        /// Returns proper SQL query part for IN operator if collection is not empty like: "Word IN ('one', 'two')"
        /// Otherwise returns empty string.
        /// </summary>
        public static string ColumnIn<TEnum>(this TEnum[] enums, string columnName) where TEnum : struct, Enum
            => enums.Select(@enum => @enum.ToString()).ColumnIn(columnName, "IN");

        /// <summary>
        /// Returns proper SQL query part for NOT IN operator if collection is not empty like: "UserId NOT IN ('57e01cad-c689-4257-88c3-9c2ef6b98982', '99994c8f-b81a-4f9c-9291-a96170ed35cf')"
        /// Otherwise returns empty string.
        /// </summary>
        public static string ColumnNotIn(this IEnumerable<Guid> ids, string columnName)
            => ids.ColumnIn(columnName, "NOT IN");

        /// <summary>
        /// Returns proper SQL query part for NOT IN operator if collection is not empty like: "Word NOT IN ('one', 'two')"
        /// Otherwise returns empty string.
        /// </summary>
        public static string ColumnNotIn(this IEnumerable<string> items, string columnName)
            => items.ColumnIn(columnName, "NOT IN");

        private static string ColumnIn(this IEnumerable<Guid> ids, string columnName, string sqlOperator) =>
            ids?.Select(id => id.ToString()).ColumnIn(columnName, sqlOperator) ?? string.Empty;

        private static string ColumnIn(this IEnumerable<string> items, string columnName, string sqlOperator)
        {
            if (items.IsNullOrEmpty())
                return string.Empty;
            
            return $"{columnName} {sqlOperator} ({string.Join(", ", items.Select(item => $"'{item}'"))})";
        }

        /// <summary>
        /// Returns proper SQL query part for = operator like: "Word = 'one'"
        /// </summary>
        public static string ColumnEquals<TEnum>(this TEnum item, string columnName) where TEnum : struct, Enum =>
             $"{columnName} = '{item}'";

        /// <summary>
        /// Returns proper SQL query part for = operator like: "Number = 0"
        /// </summary>
        public static string ColumnEquals(this object item, string columnName) =>
             $"{columnName} = {item}";

        /// <summary>
        /// Returns proper SQL query part for != operator like: "Number != 0"
        /// Otherwise returns empty string.
        /// </summary>
        public static string ColumnNotEquals(this object item, string columnName) =>
             $"{columnName} != {item}";

        /// <summary>
        /// Returns proper SQL query part for >= operator like: "Number >= 0"
        /// </summary>
        public static string ColumnMoreOrEquals(this object item, string columnName) =>
             $"{columnName} >= {item}";
    }
}