using System;
using System.IO;

namespace WX.B2C.User.Verification.DataAccess.Seed
{
    internal static class SeedConstants
    {
        private const string BasePath = @"Seed\Sources";

        public static string[] VerificationPoliciesFileNames
        {
            get
            {
                var directory = Path.Combine(BasePath, "VerificationPolicies");
                if (Directory.Exists(directory))
                    return Directory.GetFiles(directory);

                return Array.Empty<string>();
            }
        }

        public static string[] MonitoringPoliciesFileNames
        {
            get
            {
                var directory = Path.Combine(BasePath, "MonitoringPolicies");
                if (Directory.Exists(directory))
                    return Directory.GetFiles(directory);

                return Array.Empty<string>();
            }
        }

        public static string CheckVariantsFileName => Path.Combine(BasePath, "CheckVariants.json");

        public static string TicketTemplatesFileName => Path.Combine(BasePath, "TicketTemplates.json");

        public static string ValidationRulesFileName => Path.Combine(BasePath, "ValidationPolicies", "ValidationRules.json");

        public static string ValidationPoliciesFileName => Path.Combine(BasePath, "ValidationPolicies", "ValidationPolicies.json");
    }
}
