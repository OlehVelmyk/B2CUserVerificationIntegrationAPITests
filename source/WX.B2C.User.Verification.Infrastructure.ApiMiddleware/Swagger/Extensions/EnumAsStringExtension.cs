using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions
{
    internal class EnumAsStringExtension : ISwaggerExtension
    {
        private readonly string _enumName;

        public string ExtensionName => "x-ms-enum";

        public EnumAsStringExtension(string enumName) => _enumName = enumName;

        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteAny(
                    new OpenApiObject
                    {
                        ["name"] = new OpenApiString(_enumName),
                        ["modelAsString"] = new OpenApiBoolean(false)
                    }
                );
        }
    }
}
