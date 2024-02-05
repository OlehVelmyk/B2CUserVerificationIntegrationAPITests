using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Public.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Filters
{
    public class ValidateAsyncAttribute : Attribute, IFilterFactory
    {
        private readonly Type _requestType;

        public ValidateAsyncAttribute(Type requestType)
        {
            _requestType = requestType ?? throw new ArgumentNullException(nameof(requestType));
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var validatorType = typeof(RequestAsyncValidator<>).MakeGenericType(_requestType);
            var validator = serviceProvider.GetRequiredService(validatorType);

            var filterType = typeof(AsyncValidationFilter<>).MakeGenericType(_requestType);
            var filter = Activator.CreateInstance(filterType, validator);

            return (IFilterMetadata)filter;
        }

        public bool IsReusable => true;
    }

    public class AsyncValidationFilter<T> : IAsyncActionFilter
    {
        private readonly RequestAsyncValidator<T> _validator;

        public AsyncValidationFilter(RequestAsyncValidator<T> validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values.OfType<T>())
            {
                var validationResult = await _validator.ValidateAsync(argument);

                if (validationResult.IsValid)
                    continue;

                var validationErrors = validationResult.Errors
                                                       .Select(MapError)
                                                       .ToArray();

                var response = ErrorResponse.Create(validationErrors);
                context.Result = new BadRequestObjectResult(response);

                return;
            }

            await next();
        }

        /// <remarks>
        /// Be carefully with 'ValidationFailure.FormattedMessagePlaceholderValues'
        /// </remarks>
        private static ErrorDetails MapError(ValidationFailure failure) =>
            ErrorDetails.Create(MapErrorCode(failure.ErrorCode), failure.ErrorMessage);

        private static string MapErrorCode(string errorCode) =>
            errorCode.In(Constants.ErrorCodes.Defined) ? errorCode : Constants.ErrorCodes.ValidationError;
    }
}
