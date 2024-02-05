using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation
{
    public class OnlyUniqueValuesValidator<T, TProperty> : PropertyValidator<T, IEnumerable<TProperty>>
        where T : class
        where TProperty : IEquatable<TProperty>
    {
        public override string Name => "OnlyUniqueValuesValidator";

        public override bool IsValid(ValidationContext<T> context, IEnumerable<TProperty> value)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (value == null) return true;

            var hash = new HashSet<TProperty>();
            var duplicates = value.Where(i => !hash.Add(i)).ToArray();
            if (duplicates.Length == 0)
                return true;

            var formatted = string.Join(',', duplicates.Distinct());
            context.MessageFormatter.AppendArgument("Duplicates", formatted);
            return false;
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
            => "{PropertyName} must only contain unique values. Duplicate values are [{Duplicates}].";
    }
}