using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Profile
{
    public abstract class PropertyChange : ValueObject
    {
        protected PropertyChange(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public abstract bool IsReset { get; }

        public static PropertyChange<TValue> Create<TValue>(string propertyName, TValue newValue, TValue previousValue) =>
            new(propertyName, newValue, previousValue);

        public abstract void Deconstruct(out string xPath, out object newValue, out object oldValue);
    }

    public class PropertyChange<TValue> : PropertyChange
    {
        public PropertyChange(string propertyName, TValue newValue, TValue oldValue) 
            : base(propertyName)
        {
            NewValue = newValue;
            PreviousValue = oldValue;
        }

        public TValue PreviousValue { get; }

        public TValue NewValue { get; }

        public override bool IsReset => NewValue is null;

        public override void Deconstruct(out string propertyName, out object newValue, out object oldValue)
        {
            propertyName = PropertyName;
            newValue = NewValue;
            oldValue = PreviousValue;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return PropertyName;
            yield return NewValue;
            yield return PreviousValue;
        }
    }
}
