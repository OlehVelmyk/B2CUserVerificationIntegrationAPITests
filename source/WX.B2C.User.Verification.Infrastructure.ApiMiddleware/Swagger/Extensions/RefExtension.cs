using Microsoft.OpenApi;
using Microsoft.OpenApi.Writers;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions
{
    internal class RefExtension : ISwaggerExtension
    {
        private readonly string _referenceV3;

        public string ExtensionName => "$ref";

        public RefExtension(string referenceV3) => _referenceV3 = referenceV3;

        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteValue(_referenceV3);
        }
    }
}
