namespace WX.B2C.User.Verification.Api.Public.Client.Models
{
    public class FileToUpload
    {
        public FileToUpload(System.IO.Stream data, string fileName, string contentType)
        {
            Data = data;
            FileName = fileName;
            ContentType = contentType;
        }

        public System.IO.Stream Data { get; private set; }

        public string FileName { get; private set; }

        public string ContentType { get; private set; }
    }
}
