namespace WX.B2C.User.Verification.LexisNexis
{
    internal class Constants
    {
        public static class BridgerSearchModes
        {
            public const string Pep = "Political Exposure";
            public const string AdverseMedia = "Reputational Risk";
            public const string Sanction = "Sanctions Screening";
            public const string PepFamilyMembers = "PEP Family Members";
        }

        public static class RiskIndicators
        {
            public const string MissingPhoneRiskCode = "80";
            public const string MissingOrIncompleteSsnRiskCode = "79";
        }
    }
}
