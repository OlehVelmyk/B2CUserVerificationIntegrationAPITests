using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    public class DocumentHeadersFilter : IDocumentFilter
    {
        private readonly IEnumerable<OpenApiParameter> _parameters;

        public DocumentHeadersFilter(IEnumerable<OpenApiParameter> parameters)
        {
            _parameters = parameters ?? Enumerable.Empty<OpenApiParameter>();
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var parameter in _parameters)
            {
                swaggerDoc.Components.Parameters.Add(parameter.Reference.Id, parameter);
            }
        }
    }
}
