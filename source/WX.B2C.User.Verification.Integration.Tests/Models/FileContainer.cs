using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    public class FileMetaData
    {
        public string Path { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }

        public string OnfidoType { get; set; }
    }

    public abstract class FileContainer
    {
        public FileToUpload File { get; set; }

        public FileMetaData MetaData { get; set; }
    }

    public class DocumentContainer : FileContainer
    {  }

    public class LivePhotoContainer : FileContainer
    {  }
}
