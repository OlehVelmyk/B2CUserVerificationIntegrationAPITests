using WX.B2C.User.Verification.Configuration.Models;

namespace WX.B2C.User.Verification.Configuration.Seed
{
    internal class TicketsParametersMapping
    {
        public static ParametersMapping[] Seed = 
        {
            new()
            {
                Name = "UserEmail",
                Source = "PersonalDetails.Email"
            },
            new()
            {
                Name = "FirstName",
                Source = "PersonalDetails.FirstName"
            },
            new()
            {
                Name = "LastName",
                Source = "PersonalDetails.LastName"
            },
            new()
            {
                Name = "Birthdate",
                Source = "PersonalDetails.Birthdate"
            },
            new()
            {
                Name = "ResidenceCountry",
                Source = "PersonalDetails.ResidenceAddress.Country"
            },
            new()
            {
                Name = "Turnover",
                Source = "VerificationDetails.Turnover"
            },
            new()
            {
                Name = "RiskLevel",
                Source = "VerificationDetails.RiskLevel"
            },
            new()
            {
                Name = "IdDocumentNumber",
                Source = "VerificationDetails.IdDocumentNumber.Number"
            },
            new()
            {
                Name = "UtcNow",
                Source = "CurrentDateTime"
            },
            new()
            {
                Name = "OnfidoProfileLink",
                Source = "OnfidoProfileLink"
            },
            new()
            {
                Name = "PassFortLink",
                Source = "PassFortLink"
            },
            new()
            {
                Name = "CorrelationId",
                Source = "CorrelationId"
            }
        };
    }
}