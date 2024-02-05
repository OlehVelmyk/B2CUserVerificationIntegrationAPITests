using System;
using System.Collections.Generic;
using FluentValidation;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<TItem, IEnumerable<TProperty>> OnlyUniqueValues<TItem, TProperty>(
            this IRuleBuilder<TItem, IEnumerable<TProperty>> ruleBuilder)
            where TItem : class
            where TProperty : IEquatable<TProperty>
        {
            if (ruleBuilder == null)
                throw new ArgumentNullException(nameof(ruleBuilder));

            return ruleBuilder.SetValidator(new OnlyUniqueValuesValidator<TItem, TProperty>());
        }
    }
}