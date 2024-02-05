using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Utilities;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Extensions;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    public class StringEnumParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (!context.ParameterInfo.IsStringEnum())
                return;

            var factory = new StringEnumSchemaFactory(context.SchemaRepository, context.SchemaGenerator);
            parameter.Schema = factory.Create(parameter.Schema, context.ParameterInfo);
            parameter.Name = context.ParameterInfo.Name;
        }
    }
}
