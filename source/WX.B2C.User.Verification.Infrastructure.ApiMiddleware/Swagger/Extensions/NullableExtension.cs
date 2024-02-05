using Microsoft.OpenApi;
using Microsoft.OpenApi.Writers;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions
{
    internal class NullableExtension : ISwaggerExtension
    {
        private readonly bool _isNullable;

        public string ExtensionName => "x-nullable";

        public NullableExtension(bool isNullable) => _isNullable = isNullable;

        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteRaw(_isNullable.ToString().ToLower());
        }
    }
}
