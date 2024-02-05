using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    /// <summary>
    /// All schemas is not nullable and all properties is required by default
    /// </summary>
    public class RequiredSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Nullable = false;

            if (schema.Type != "object")
                return;

            foreach (var property in schema.Properties)
                schema.Required.Add(property.Key);
        }
    }
}
