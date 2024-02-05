using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Domain.Profile;

namespace WX.B2C.User.Verification.Unit.Tests.Extensions
{
    internal static class PropertyChangeExtensions
    {
        public static bool ContainsOnly(this IEnumerable<PropertyChange> changes, string propertyName) =>
            changes.ContainsOnly(new[] { propertyName });

        public static bool ContainsOnly(this IEnumerable<PropertyChange> changes, IEnumerable<string> propertyNames) =>
            changes.Select(change => change.PropertyName).All(propertyNames.Contains);

        public static PropertyChange<T> Find<T>(this IEnumerable<PropertyChange> changes, string propertyName) =>
            (PropertyChange<T>) changes.FirstOrDefault(change => change.PropertyName == propertyName);
    }
}