namespace WX.B2C.User.Verification.Component.Tests.Constants
{
    internal static class ValidationRules
    {
        public const int MaxFileSize = 1024 * 1024 * 10;

        public static string[] FileExtensions { get; } = { "png", "jpeg", "jpg", "pdf", "mp4" };
    }
}