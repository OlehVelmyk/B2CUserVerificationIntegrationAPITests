using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Events.Internal.Dtos;

namespace WX.B2C.User.Verification.Events.Internal.Extensions
{
    public static class PropertyChangeExtensions
    {
        public static PropertyChange<T> Find<T>(this IEnumerable<PropertyChangeDto> changes, string propertyName)
        {
            if (changes == null)
                throw new ArgumentNullException(nameof(changes));
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var propertyChange = changes.FirstOrDefault(x => x.PropertyName == propertyName);
            if (propertyChange == null)
                return null;

            var newValue = PropertyChangeSerializer.Deserialize<T>(propertyChange.NewValue);
            var previousValue = PropertyChangeSerializer.Deserialize<T>(propertyChange.PreviousValue);
            return new PropertyChange<T>(newValue, previousValue);
        }
    }

    public class PropertyChange<T>
    {
        public PropertyChange(T newValue, T previousValue)
        {
            NewValue = newValue;
            PreviousValue = previousValue;
        }

        public T PreviousValue { get; }

        public T NewValue { get; }
    }
}
