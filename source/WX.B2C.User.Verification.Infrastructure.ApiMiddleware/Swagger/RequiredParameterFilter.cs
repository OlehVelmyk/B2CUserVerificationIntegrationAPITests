using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    public class RequiredParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            var parameterInfo = (ICustomAttributeProvider)context.PropertyInfo ?? context.ParameterInfo;
            if (parameterInfo is null) return;

            var isOptional = IsOptional(parameterInfo);

            parameter.Required = !isOptional;
            parameter.AllowEmptyValue = isOptional;
        }

        private static bool IsOptional(ICustomAttributeProvider parameterInfo) =>
            parameterInfo.GetCustomAttributes(typeof(NotRequiredAttribute), false)
                         .Length > 0;
    }
}