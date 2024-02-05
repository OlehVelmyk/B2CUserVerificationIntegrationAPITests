using FsCheck;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators
{
    internal static class ReferrerGenerator
    {
        private static string[] ReferrerTemplates = {
            "http://{DOMAIN}.com/",
            "http://{SUBDOMAIN}.{DOMAIN}.com/",
            "https://{DOMAIN}.com/",
            "https://{SUBDOMAIN}.{DOMAIN}.com/",
            "http://*.{DOMAIN}.com/",
            "https://*.{DOMAIN}.com/",
            "http://{DOMAIN}.com/{PATH}/",
            "https://*.{DOMAIN}.com/{PATH}/",
            "http://{DOMAIN}.com/{PATH}/*",
            "https://*.{DOMAIN}.com/{PATH}/*"
        };

        private static Gen<string> ReferrerTemplate = Gen.Elements(ReferrerTemplates);

        private static string[] InvalidReferrerTemplates = {  
            "http",
            "https",
            "{DOMAIN}.com",
            "*{DOMAIN}.com",
            "*.{DOMAIN}.com",
            "{PATH}/*"
        };

        private static readonly Gen<string> InvalidReferrerTemplate = Gen.Elements(InvalidReferrerTemplates);

        public static Gen<string> Referrer(int maxLength) =>
            from domain in StringGenerators.LettersOnly(1, maxLength)
            from subdomain in StringGenerators.LettersOnly(1, maxLength)
            from path in StringGenerators.LettersOnly(1, maxLength)
            from template in ReferrerTemplate
            select template.Replace("{DOMAIN}", domain)
                           .Replace("{SUBDOMAIN}", subdomain)
                           .Replace("{PATH}", path);

        public static Gen<string> InvalidReferrer(int maxLength) =>
            from domain in StringGenerators.LettersOnly(1, maxLength)
            from path in StringGenerators.LettersOnly(1, maxLength)
            from template in InvalidReferrerTemplate
            select template.Replace("{DOMAIN}", domain)
                           .Replace("{PATH}", path);
    }
}
