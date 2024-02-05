using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Profile;

namespace WX.B2C.User.Verification.PassFort
{
    public static class Constants
    {
        public static class CheckTypes
        {
            public const string PepAndSanctionsScreen = "PEPS_AND_SANCTIONS_SCREEN";
        }

        public static class ProfileRoles
        {
            public const string IndividualCustomer = "INDIVIDUAL_CUSTOMER";
        }

        public static class CollectedDataTypes
        {
            public const string Individual = "INDIVIDUAL";
        }

        public static class Tags
        {
            public const string ApacResidenceTag = "Wirex Singapore";
            public const string USResidenceTag = "FATCA";
            public const string AccountClosureTag = "AccountClosure";

            public static class RiskLevelTags
            {
                public const string LowTag = "LowRiskRating";
                public const string MediumTag = "MediumRiskRating";
                public const string HighTag = "HighRiskRating";
                public const string ExtraHighTag = "ExtraHighRiskLevel";

                public static readonly IEnumerable<string> AllTags = new[]
                {
                    LowTag,
                    MediumTag,
                    HighTag,
                    ExtraHighTag
                };

                public static string GetTag(RiskLevel riskLevel)
                {
                    return riskLevel switch
                    {
                        RiskLevel.Low       => LowTag,
                        RiskLevel.Medium    => MediumTag,
                        RiskLevel.High      => HighTag,
                        RiskLevel.ExtraHigh => ExtraHighTag,
                        _                   => throw new ArgumentOutOfRangeException(nameof(riskLevel), riskLevel, null)
                    };
                }
            }

            public const string OnfidoIdTagPrefix = "onfido: ";

            public static string GetOnfidoTag(string onfidoId)
            {
                return $"{OnfidoIdTagPrefix}{onfidoId}";
            }
        }
        
        public static class ApplicationState
        {
            public const string Applied = "APPLIED";
            public const string Approved = "APPROVED";
            public const string Cancelled = "CANCELLED";
            public const string Rejected = "REJECTED";
            public const string InReview = "IN_REVIEW";
        }
    }
}