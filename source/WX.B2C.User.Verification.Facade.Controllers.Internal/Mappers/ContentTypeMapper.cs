using Microsoft.AspNetCore.StaticFiles;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers
{
    public interface IContentTypeMapper
    {
        string Map(string fileName);
    }

    public class ContentTypeMapper : IContentTypeMapper
    {
        private const string ApplicationOctetStream = "application/octet-stream";
        private readonly FileExtensionContentTypeProvider _provider;

        public ContentTypeMapper()
        {
            _provider = new FileExtensionContentTypeProvider();
        }

        public string Map(string fileName)
        {
            return !_provider.TryGetContentType(fileName, out var contentType) ? ApplicationOctetStream : contentType;
        }
    }
}