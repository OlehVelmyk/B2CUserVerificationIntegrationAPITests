using WX.B2C.User.Verification.Configuration.Models;

namespace WX.B2C.User.Verification.Configuration.Seed
{
    internal static class Tickets
    {
        public static Ticket[] Seed =
        {
            new()
            {
                Reason = "poa-doc-verification-needed",
                Parameters = new[]
                {
                    "ResidenceCountry"
                }
            },
            new()
            {
                Reason = "poi-check-result-review-needed",
                Parameters = new[]
                {
                    "UserEmail",
                    "FirstName",
                    "LastName",
                    "OnfidoProfileLink"
                }
            },
            new()
            {
                Reason = "sof-doc-verification-needed",
                Parameters = new[]
                {
                    "FirstName",
                    "LastName"
                }
            },
            new()
            {
                Reason = "sof-survey-review-needed"
            },
            new()
            {
                Reason = "w-9-form-review-needed"
            },
            new()
            {
                Reason = "first-threshold-reached",
                Parameters = new[]
                {
                    "UtcNow",
                    "Turnover",
                    "RiskLevel"
                },
                Formats = new() { { "UtcNow", "F" } }
            },
            new()
            {
                Reason = "second-threshold-reached",
                Parameters = new[]
                {
                    "UtcNow",
                    "Turnover",
                    "RiskLevel"
                },
                Formats = new()
                {
                    { "UtcNow", "F" }
                }
            },
            new()
            {
                Reason = "repeating-turnover-threshold-reached",
                Parameters = new[]
                {
                    "UtcNow",
                    "Turnover",
                    "RiskLevel"
                },
                Formats = new()
                {
                    { "UtcNow", "F" }
                }
            },
            new()
            {
                Reason = "account-alert-created",
                Parameters = new[]
                {
                    "Turnover",
                    "RiskLevel",
                    "ApplicationState",
                    "LastApprovedDate"
                },
                Formats = new()
                {
                    { "LastApprovedDate", "F" }
                }
            },
            new()
            {
                Reason = "usa-edd-check-list-review-needed"
            },
            new()
            {
                Reason = "pass-fort-risk-screening-failed",
                Parameters = new[]
                {
                    "FirstName",
                    "LastName",
                    "PassFortLink",
                    "UserEmail"
                }
            },
            new()
            {
                Reason = "lexis-nexis-risk-screening-failed",
                Parameters = new[]
                {
                    "FirstName",
                    "LastName",
                    "UserEmail"
                }
            },
            new()
            {
                Reason = "usa-onfido-check-failed",
                Parameters = new[]
                {
                    "FirstName",
                    "LastName",
                    "UserEmail",
                    "OnfidoProfileLink"
                }
            },
            new()
            {
                Reason = "onfido-known-faces-image-quality-check-failed",
                Parameters = new[]
                {
                    "FirstName",
                    "LastName",
                    "UserEmail"
                }
            },
            new()
            {
                Reason = "additional-docs-review-needed",
                Parameters = new[]
                {
                    "FirstName",
                    "LastName",
                    "UserEmail",
                    "PassFortLink"
                }
            },
            new()
            {
                Reason = "instant-id-check-failed",
                Parameters = new[]
                {
                    "FirstName",
                    "LastName",
                    "UserEmail"
                }
            },
            new()
            {
                Reason = "pep-monthly-review-reminder",
                Parameters = new[]
                {
                    "Turnover",
                    "RiskLevel"
                }
            },
            new()
            {
                Reason = "name-and-dob-duplication-detected",
                Parameters = new[]
                {
                    "FirstName",
                    "LastName",
                    "Birthdate"
                },
                Formats = new()
                {
                    { "Birthdate", "d" }
                }
            },
            new()
            {
                Reason = "id-doc-number-duplication-detected",
                Parameters = new[]
                {
                    "IdDocumentNumber"
                }
            },
            new()
            {
                Reason = "usa-ip-match-failed",
                Parameters = new[]
                {
                    "FirstName",
                    "LastName",
                    "UserEmail"
                }
            },
            new()
            {
                Reason = "ip-match-failed",
                Parameters = new[]
                {
                    "CorrelationId"
                }
            },
            new()
            {
                Reason = "check-error-detected",
                Parameters = new[]
                {
                    "ApplicationState",
                    "CheckType",
                    "CorrelationId"
                }
            },
            new()
            {
                Reason = "poi-attempts-exceeded",
                Parameters = new string[0]
            }
        };
    }
}