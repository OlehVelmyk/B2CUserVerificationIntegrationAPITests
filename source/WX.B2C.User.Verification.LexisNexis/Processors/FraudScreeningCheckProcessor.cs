using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WX.B2C.User.Verification.LexisNexis.Models;
using WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.LexisNexis.Processors
{
    internal sealed class FraudScreeningCheckProcessor
    {
        public CheckProcessingResult Process(InitiateWorkflowResponse response)
        {
            if (response == null)
                throw PropertyValidationException(nameof(response));

            if (response.Status?.TransactionStatus == "error")
                throw new CheckProcessingException(Provider.Contracts.Constants.ErrorCodes.ProviderUnknownError, "Error transaction status received.", MapErrors(response));

            var comprehensiveVerification = GetComprehensiveVerification(response.PassThroughs);

            return Estimate(comprehensiveVerification);
        }

        private static IDictionary<string, object> MapErrors(InitiateWorkflowResponse response)
        {
            return response.Information?.SelectMany(
                               info => info?.DetailDescription,
                               (info, description) => new
                               {
                                   info.Code,
                                   description?.Text
                               })
                           .ToDictionary(
                               error => error.Code,
                               error => (object)error.Text);
        }

        private static ComprehensiveVerification GetComprehensiveVerification(IEnumerable<PassThrough> passThroughs)
        {
            if (passThroughs == null)
                throw PropertyValidationException(nameof(passThroughs));

            var passThrough = GetInstantIdPassThrough(passThroughs);
            if (passThrough.Data == null)
                throw PropertyValidationException($"{nameof(passThrough)}.{nameof(passThrough.Data)}");

            var instantIdData = DeserializeResponse(passThrough.Data);
            return GetComprehensiveVerification(instantIdData?.InstantIDResponseEx);
        }

        private static PassThrough GetInstantIdPassThrough(IEnumerable<PassThrough> passThroughs)
        {
            var passThrough = passThroughs.FirstOrDefault(x => x is { Type: "INSTANT_ID" });
            if (passThrough == null)
                throw ValidationException("No PassThrough with type INSTANT_ID");

            return passThrough;
        }

        private static ComprehensiveVerification GetComprehensiveVerification(InstantIDResponseEx instantIdData)
        {
            if (instantIdData == null)
                throw PropertyValidationException(nameof(instantIdData));

            var response = instantIdData.Response;
            if (response == null)
                throw PropertyValidationException($"{nameof(instantIdData)}.{nameof(response)}");

            var result = response.Result;
            if (result == null)
                throw PropertyValidationException($"{nameof(instantIdData)}.{nameof(response)}.{nameof(result)}");

            var comprehensiveVerification = result.ComprehensiveVerification;
            if (comprehensiveVerification == null)
                throw PropertyValidationException(nameof(comprehensiveVerification));

            return comprehensiveVerification;
        }

        private static CheckProcessingResult Estimate(ComprehensiveVerification comprehensiveVerification)
        {
            var outputData = Map(comprehensiveVerification);

            if (comprehensiveVerification.ComprehensiveVerificationIndex.GetValueOrDefault(0) < 30)
                return CheckProcessingResult.Failed(outputData, Domain.Checks.CheckDecisions.InstantIdClosing);

            var riskIndicators = comprehensiveVerification.RiskIndicators?.RiskIndicator;
            if (comprehensiveVerification.ComprehensiveVerificationIndex < 40 || riskIndicators.HasPartialRiskIndicators())
                return CheckProcessingResult.Failed(outputData, Domain.Checks.CheckDecisions.PotentialFraud);

            return CheckProcessingResult.Passed(outputData);
        }

        private static LexisNexisFraudScreeningOutputData Map(ComprehensiveVerification comprehensiveVerification)
        {
            if (comprehensiveVerification == null)
                throw new ArgumentNullException(nameof(comprehensiveVerification));

            var riskIndicators = comprehensiveVerification
                                 .RiskIndicators
                                 ?.RiskIndicator
                                 ?.Select(x => new LexisNexisFraudScreeningOutputData.RiskIndicator
                                 {
                                     RiskCode = x.RiskCode,
                                     Description = x.Description
                                 })
                                 .ToArray();

            return new LexisNexisFraudScreeningOutputData
            {
                ComprehensiveIndex = comprehensiveVerification.ComprehensiveVerificationIndex,
                RiskIndicators = riskIndicators
            };
        }

        private static InstantIDResponseExWrapper DeserializeResponse(string data)
        {
            try
            {
                return JsonConvert.DeserializeObject<InstantIDResponseExWrapper>(data);
            }
            catch
            {
                var additionalData = new Dictionary<string, object> { [nameof(data)] = data };
                throw new CheckProcessingException(Provider.Contracts.Constants.ErrorCodes.ProviderInvalidResponse, "PassThrough data deserialization failed.", additionalData);
            }
        }

        private static CheckProcessingException PropertyValidationException(string propertyName) =>
            ValidationException($"Property {propertyName} is invalid.");

        private static CheckProcessingException ValidationException(string message) =>
            new CheckProcessingException(Provider.Contracts.Constants.ErrorCodes.ProviderInvalidResponse, message);
    }
}
