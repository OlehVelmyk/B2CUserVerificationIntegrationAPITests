using Autofac;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Filters;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Api.Admin.Extensions
{
    internal static class FluentValidationExtensions
    {
        public static IMvcBuilder AddFluentValidation(this IMvcBuilder mvcBuilder) =>
            mvcBuilder.AddFluentValidation(fluentConfig =>
            {
                fluentConfig.RegisterValidatorsFromAssemblyContaining<ValidationFilter>(
                discoveredType => !discoveredType.ValidatorType.IsClosedTypeOf(typeof(RequestAsyncValidator<>)));
            });
    }
}