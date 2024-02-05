using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Extensions;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Utilities;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    /// <summary>
    /// Customize response.
    /// </summary>
    public class ResponseBodyFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodInfo = context.MethodInfo;
            if (methodInfo == null)
                return;

            var schemaProcessor = new SchemaProcessor(context.SchemaRepository, context.SchemaGenerator);

            foreach (var response in operation.Responses.Values)
            {
                if (!TryGetResponseType(response, methodInfo, out var responseType))
                    continue;

                if (response.Content != null && response.Content.Any())
                {
                    foreach (var mediaType in response.Content.Values)
                    {
                        var responseSchema = mediaType.Schema;
                        schemaProcessor.Process(responseSchema, responseType);
                    }
                }
            }
        }

        private bool TryGetResponseType(OpenApiResponse response, MethodInfo methodInfo, out Type responseType)
        {
            responseType = null;

            if (response.Description == "Error")
            {
                var defaultResponseAttribute = methodInfo.GetCustomAttribute<ProducesDefaultResponseTypeAttribute>();
                if (defaultResponseAttribute != null)
                    responseType = defaultResponseAttribute.Type;
            }
            else if (response.Description == "Success")
            {
                responseType = methodInfo.ReturnParameter.ParameterType;
                if (responseType.IsTask() && responseType.IsGenericType)
                    responseType = responseType.FindGenericArgument(0);
                if (responseType.IsActionResult() && responseType.IsGenericType)
                    responseType = responseType.FindGenericArgument(0);
            }

            return responseType != null;
        }
    }
}
