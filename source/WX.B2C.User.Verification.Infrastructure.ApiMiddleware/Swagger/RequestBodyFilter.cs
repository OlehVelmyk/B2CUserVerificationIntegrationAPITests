using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Utilities;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    /// <summary>
    /// Make body in all operations required by default + customize request.
    /// </summary>
    public class RequestBodyFilter : IRequestBodyFilter
    {
        public void Apply(OpenApiRequestBody requestBody, RequestBodyFilterContext context)
        {
            if (context.BodyParameterDescription == null)
                return;

            requestBody.Required = true;
            
            var schemaProcessor = new SchemaProcessor(context.SchemaRepository, context.SchemaGenerator);

            var body = context.BodyParameterDescription.ParameterInfo();
            var bodyType = body.ParameterType;
            
            foreach (var mediaType in requestBody.Content.Values)
            {
                var bodySchema = mediaType.Schema;
                schemaProcessor.Process(bodySchema, bodyType);
            }
        }
    }
}