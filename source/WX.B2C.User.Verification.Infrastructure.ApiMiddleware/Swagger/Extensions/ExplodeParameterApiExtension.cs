using Microsoft.OpenApi;
using Microsoft.OpenApi.Writers;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions
{
    internal class ExplodeParameterApiExtension : ISwaggerExtension
    {
        public string ExtensionName => "explode";

        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteRaw("true");
        }
    }
}
