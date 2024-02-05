using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    public class ParameterDuplicationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null || operation.Parameters.Count < 2)
                return;

            var parameters = new List<OpenApiParameter>();
            foreach (var parameter in operation.Parameters)
                if (string.IsNullOrEmpty(parameter.Name) || !parameters.Any(p => p.Name == parameter.Name))
                    parameters.Add(parameter);

            if (operation.Parameters.Count != parameters.Count)
                operation.Parameters = parameters;
        }
    }
}
