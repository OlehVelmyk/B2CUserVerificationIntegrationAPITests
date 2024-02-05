using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WX.B2C.User.Verification.PassFort.Client.Models;
using WX.B2C.User.Verification.PassFort.Models;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using ErrorCodes = WX.B2C.User.Verification.Provider.Contracts.Constants.ErrorCodes;

namespace WX.B2C.User.Verification.PassFort.Processors
{
    internal interface ICheckProcessor
    {
        CheckProcessingResult Process(CheckResourcePepsAndSanctionsScreen check);
    }

    internal class PassFortCheckProcessor : ICheckProcessor
    {
        public CheckProcessingResult Process(CheckResourcePepsAndSanctionsScreen check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            var matches = JsonSerializer.Serialize(check.OutputData.SanctionsResults);
            var outputData = new RiskScreeningCheckOutputData { Matches = matches };

            if (check.Result == CheckResult.Pass)
                return CheckProcessingResult.Passed(outputData);

            if (check.Result == CheckResult.Error)
            {
                var errors = check.Errors?.Select(FormatPassFortError).ToArray();

                throw new CheckProcessingException(
                    ErrorCodes.ProviderUnknownError,
                    "PassFort check exception.",
                    new Dictionary<string, object> { [nameof(check.Errors)] = errors });
            }

            outputData.IsPep = check.Result == CheckResult.PEP;
            outputData.IsSanctioned = check.Result == CheckResult.Sanction;
            outputData.IsAdverseMedia = check.Result == CheckResult.Media || check.Result == CheckResult.Refer;

            return CheckProcessingResult.Failed(outputData);

            static string FormatPassFortError(Error error) =>
                $"Code: {error.Code}, Message: {error.Message}, Source: {error.Source}";
        }
    }
}
