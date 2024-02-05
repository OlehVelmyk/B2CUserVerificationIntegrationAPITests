using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    // Creating of not nullable array can be made on schema level
    // But to prevent complicating of all next operations 
    // with schemas without reference (because assign 'null' to property 'Reference' here) 
    // it made on document level
    public class NotNullableArrayFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var parametersSchemas = swaggerDoc.Paths.Values
                .SelectMany(path => path.Operations.Values)
                .SelectMany(operation => operation.Parameters)
                .Select(parameter => parameter.Schema);
            var propertiesSchemas = context.SchemaRepository.Schemas.Values.SelectMany(s => s.Properties.Values);

            parametersSchemas
                .Concat(propertiesSchemas)
                .Where(schema => schema != null && schema.Type == "array")
                .Foreach(schema =>
                {
                    schema.Items.Nullable = false;
                    schema.Items.Extensions.Add(new NullableExtension(false));

                    // '$ref' overwrites any sibling keywords so anyway use '$ref' but as extension
                    if (schema.Items.Reference != null)
                    {
                        var refExtension = new RefExtension(schema.Items.Reference.ReferenceV3);
                        schema.Items.Extensions.Add(refExtension);
                        schema.Items.Reference = null;
                    }
                });
        }
    }
}
