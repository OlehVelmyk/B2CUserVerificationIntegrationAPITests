using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    public class RequiredHeadersFilter : IOperationFilter
    {
        private readonly IEnumerable<OpenApiReference> _references;

        public RequiredHeadersFilter(IEnumerable<OpenApiReference> references)
        {
            _references = references ?? Enumerable.Empty<OpenApiReference>();
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var reference in _references)
            {
                var parameter = new OpenApiParameter { Reference = reference };
                operation.Parameters.Add(parameter);
            }
        }
    }
}
