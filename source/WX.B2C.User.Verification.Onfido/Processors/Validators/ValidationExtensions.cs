using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Onfido.Processors.Validators
{
    internal static class ValidationExtensions
    {
        private const string Errors = nameof(Errors);

        public static bool IsClearResult(this ReportResult? result) =>
            result == null || result == ReportResult.Clear;

        public static bool IsClearResult(this ReportSubResult? result) =>
            result == null || result == ReportSubResult.Clear;

        public static bool IsClearResult(this BreakdownResult breakdown) =>
            breakdown?.Result == null || breakdown.Result == ReportSubResult.Clear;

        public static string GetDecision(this ValidationResult validationResult)
        {
            if (validationResult == null)
                throw new ArgumentNullException(nameof(validationResult));

            return validationResult.Errors?
                                   .Where(x => x.ErrorCode != null)
                                   .Select(x => x.ErrorCode)
                                   .FirstOrDefault();
        }

        public static object[] GetFailures(this ValidationResult validationResult)
        {
            if (validationResult == null)
                throw new ArgumentNullException(nameof(validationResult));

            return validationResult.Errors?
                                   .Select(failure => new
                                   {
                                       failure.PropertyName,
                                       failure.CustomState
                                   })
                                   .ToArray<object>();
        }

        public static IRuleBuilderOptions<TReport, TBreakdown> ValidateBreakdown<TReport, TBreakdown>(
            this IRuleBuilder<TReport, TBreakdown> ruleBuilder,
            Func<TBreakdown, IList<string>> validator)
        {
            if (ruleBuilder == null)
                throw new ArgumentNullException(nameof(ruleBuilder));
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            return ruleBuilder.Must((report, breakdown, context) =>
            {
                var errors = validator(breakdown);
                if (errors.Count == 0)
                    return true;

                context.RootContextData.Add(Errors, errors.ToArray());
                return false;
            });
        }

        public static IRuleBuilderOptions<T, TProperty> WithErrorState<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule) => rule.WithState(Errors);

        private static IRuleBuilderOptions<T, TProperty> WithState<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule, string name)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(rule));

            var configurableRule = DefaultValidatorOptions.Configurable(rule);
            configurableRule.Current.CustomStateProvider = (context, _) =>
                context.RootContextData.TryGetValue(name, out var value) ? value : null;

            return rule;
        }
    }
}