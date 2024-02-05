using Microsoft.OpenApi.Interfaces;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions
{
    internal interface ISwaggerExtension : IOpenApiExtension
    {
        string ExtensionName { get; }
    }
}
