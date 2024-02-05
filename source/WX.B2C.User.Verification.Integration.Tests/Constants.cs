using System.IO;

namespace WX.B2C.User.Verification.Integration.Tests
{
    public static class Constants
    {
        public static readonly string RootPath = Directory.GetCurrentDirectory();
        public const string SettingsFilePath = "appsettings.json";

        public static class KeyVault
        {
            public const string UrlPath = "KeyVault:KeyVaultUrl";
            public const string ClientIdPath = "KeyVault:KeyVaultClientId";
            public const string SecretPath = "KeyVault:KeyVaultSecret";
        }

        public static class Content
        {
            public const string Folder = "Content";

            public const string DrivingLicence = "sample_driving_licence.png";
            public const string Passport = "sample_passport.jpg";
            public const string Photo1 = "sample_photo1.png";
            public const string Photo2 = "sample_photo2.jpg";
        }

        public static class Arbitrary
        {
            public const string SourcesFolder = "Arbitraries/Sources";

            public const string DynamicTasks = "DynamicTasks.json";
        }
        
        public static class LexisNexis
        {
            public const string SectionName = "LexisNexis";

            public static class BridgerConfiguration
            {
                public const string BridgerSectionName = "Bridger";
                public const string PasswordPath = "Password";
                public const string UserIdPath = "UserId";
            }
            
            public static class BridgerSearchModes
            {
                public const string Pep = "Political Exposure";
                public const string AdverseMedia = "Reputational Risk";
                public const string Sanction = "Sanctions Screening";
            }
        }

        public static class OnfidoFileType
        {
            public const string Passport = "passport";
            public const string DrivingLicence = "driving_licence";
        }
    }
}
