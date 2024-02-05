using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions
{
    internal static class OpenApiExtensions
    {
        public static void Add(this IDictionary<string, IOpenApiExtension> dictionary, ISwaggerExtension extension)
        {
            if(!dictionary.TryAdd(extension.ExtensionName, extension))
                dictionary[extension.ExtensionName] = extension;
        }
    }
}
