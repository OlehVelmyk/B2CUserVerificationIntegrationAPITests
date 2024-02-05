using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Extensions
{
    internal static class FluentValidationExtensions
    {
        public static void Failure<T>(this ValidationContext<T> context, string description, string errorCode) =>
            Failure(context, context.PropertyName, description, errorCode);

        public static void Failure<T>(this ValidationContext<T> context, string property, string description, string errorCode) =>
            context.AddFailure(new ValidationFailure(property, description) { ErrorCode = errorCode });

        public static void Failure<T>(this ValidationContext<T> context, string description, string errorCode, Dictionary<string,object> parameters) =>
            context.AddFailure(new ValidationFailure(context.PropertyName, description) { ErrorCode = errorCode, FormattedMessagePlaceholderValues = parameters });
    }
}