using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using Tasks = WX.B2C.User.Verification.Extensions.TaskExtensions;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Onfido.Processors;

namespace WX.B2C.User.Verification.Onfido.Sandbox
{
    internal class IdentityDocumentProcessorDecorator : IIdentityDocumentCheckResultProcessor
    {
        private readonly IIdentityDocumentCheckResultProcessor _inner;
        private readonly IProfileStorage _profileStorage;

        public IdentityDocumentProcessorDecorator(IProfileStorage profileStorage, IIdentityDocumentCheckResultProcessor inner)
        {
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public async Task<CheckProcessingResult> ProcessAsync(Check check, IList<Report> reports)
        {
            var (processingResult, idDocNumber) = await Tasks.WhenAll(
                _inner.ProcessAsync(check, reports),
                GetIdDocumentNumberAsync(check.ApplicantId));

            if (processingResult.OutputData is not IdentityCheckOutputData output)
                throw new InvalidOperationException($"Only {nameof(IdentityCheckOutputData)} can be decorated.");

            var idDocumentNumber = output.IdDocumentNumber;
            if (idDocumentNumber is not null && idDocumentNumber != IdDocumentNumberDto.NotPresented)
                idDocumentNumber.Number = idDocNumber;

            return processingResult;
        }

        private async Task<string> GetIdDocumentNumberAsync(string applicantId)
        {
            var personalDetails = await _profileStorage.GetPersonalDetailsByExternalProfileIdAsync(applicantId);
            var address = personalDetails.ResidenceAddress;
            if (address is null)
                throw new InvalidOperationException($"{nameof(personalDetails.ResidenceAddress)} is mandatory to decorate {nameof(IdentityCheckOutputData)}.");

            const string IdDocPattern = @"^IdDoc:(?<number>(\d|[A-Z]){4,20})$";
            var input = address.Line2 ?? string.Empty;
            var match = Regex.Match(input, IdDocPattern);

            var group = match.Groups["number"];
            return group.Success ? group.Value : GenerateIdDocNumber();
        }

        private static string GenerateIdDocNumber()
        {
            var rnd = new Random();
            var length = rnd.Next(8, 15);
            var charArray = Enumerable.Range(0, length).Select(_ => (char)rnd.Next(48, 57)).ToArray();
            return new string(charArray);
        }
    }
}
