using System.IO;

namespace WX.B2C.User.Verification.Component.Tests.Constants
{
    internal static class Content
    {
        private const string Folder = "Content";
        public static readonly string PathToFolder = Path.Combine(Global.RootPath, Folder);

        public const string PngFile = "sample.png";
        public const string JpgFile = "sample.jpg";
        public const string PdfFile = "sample.pdf";
        public const string Mp4File = "sample.mp4";
    }
}