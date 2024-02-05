using System.IO;

namespace WX.B2C.User.Verification.Component.Tests.Constants
{
    internal static class Sources
    {
        public const string Folder = nameof(Sources);
        public static readonly string CheckInfos = $"{nameof(CheckInfos)}.json";
        public static readonly string PathToCheckInfos = Path.Combine(Global.RootPath, Folder, CheckInfos);
    }
}
