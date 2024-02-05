using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            var enumName = type.Name;

            if (type.IsEnum)
                schema.Extensions.Add(new EnumAsStringExtension(enumName));
        }
    }
}
