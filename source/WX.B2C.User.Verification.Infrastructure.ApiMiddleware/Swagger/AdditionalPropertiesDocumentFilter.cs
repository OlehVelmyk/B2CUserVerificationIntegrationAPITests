using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    /// <summary>
    /// https://angela-evans.com/fix-autorest-problems-when-generating-client-with-swashbuckle-swagger-json/
    /// </summary>
    public class AdditionalPropertiesDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument openApiDoc, DocumentFilterContext context)
        {
            foreach (var schema in context.SchemaRepository.Schemas
                                          .Where(schema => schema.Value.AdditionalProperties == null))
            {
                schema.Value.AdditionalPropertiesAllowed = true;
            }
        }
    }
}