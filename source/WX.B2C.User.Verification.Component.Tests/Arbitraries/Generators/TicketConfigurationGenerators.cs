using System.Collections.Generic;
using System.Linq;
using FsCheck;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal static class TicketConfigurationGenerators
    {
        private static readonly string[] ParameterNames =
        {
            "UtcNow",
            "Turnover",
            "RiskLevel",
            "UserEmail",
            "FirstName",
            "LastName",
            "OnfidoProfileLink",
            "ResidenceCountry",
            "ApplicationState",
            "LastApprovedDate",
            "PassFortLink",
            "IdDocumentNumber",
            "CorrelationId"
        };

        private static readonly string[] ParameterSources =
        {
            "PersonalDetails.Email",
            "PersonalDetails.FirstName",
            "PersonalDetails.LastName",
            "PersonalDetails.Birthdate",
            "PersonalDetails.ResidenceAddress.Country",
            "VerificationDetails.Turnover",
            "VerificationDetails.RiskLevel",
            "VerificationDetails.IdDocumentNumber.Number",
            "CurrentDateTime",
            "OnfidoProfileLink",
            "PassFortLink",
            "CorrelationId"
        };

        private static readonly KeyValuePair<string, string>[] ParametersFormats =
        {
            new("Birthdate", "d"),
            new("LastApprovedDate", "F"),
            new("UtcNow", "F")
        };

        public static Gen<string[]> Parameters =>
            from parameters in Gen.Elements(ParameterNames).ArrayOf()
            select parameters.Distinct().ToArray();

        public static Gen<ParametersMappingDto[]> ParametersMapping(string[] parametersNames) =>
            from sources in Gen.Elements(ParameterSources).ArrayOf(parametersNames.Length)
            select parametersNames.Zip(sources).Select(tuple => new ParametersMappingDto
            {
                Name = tuple.First,
                Source = tuple.Second
            }).ToArray();

        public static Gen<TicketDto> Ticket(string[] parameters) =>
            from reason in StringGenerators.NotEmpty(100)
            from formats in ParameterFormatsGenerator(parameters)
            select new TicketDto
            {
                Reason = reason,
                Parameters = parameters?.Distinct().ToList(),
                Formats = formats
            };

        private static Gen<IDictionary<string, string>> ParameterFormatsGenerator(string[] parameters)
        {
            IDictionary<string, string> result = null;

            if (!parameters.IsNullOrEmpty())
            {
                result = parameters
                         .GroupJoin(ParametersFormats,
                             parameter => parameter,
                             parameterFormats => parameterFormats.Key,
                             (name, formats) => new
                             {
                                 Name = name,
                                 Formats = formats.Select(format => format.Value)
                             })
                         .Where(parameter => parameter.Formats.Any())
                         .ToDictionary(
                             parameter => parameter.Name,
                             parameter => parameter.Formats.FirstOrDefault());
            }

            return Gen.Constant(result);
        }
    }
}
