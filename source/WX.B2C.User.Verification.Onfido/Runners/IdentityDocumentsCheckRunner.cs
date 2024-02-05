using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Configurations;
using WX.B2C.User.Verification.Onfido.Extensions;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Onfido.Processors;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using ErrorCodes = WX.B2C.User.Verification.Provider.Contracts.Constants.ErrorCodes;

namespace WX.B2C.User.Verification.Onfido.Runners
{
    internal sealed class IdentityDocumentsCheckRunner : OnfidoCheckRunner<IdentityDocumentsCheckInputData>
    {
        private readonly IdentityDocumentsCheckConfiguration _configuration;
        private readonly IIdentityDocumentCheckResultProcessor _checkResultProcessor;

        public IdentityDocumentsCheckRunner(
            IdentityDocumentsCheckConfiguration configuration,
            IOnfidoApiClientFactory clientFactory,
            IIdentityDocumentCheckResultProcessor checkResultProcessor) : base(clientFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _checkResultProcessor = checkResultProcessor ?? throw new ArgumentNullException(nameof(checkResultProcessor));
        }

        protected override Task<Applicant> UpdateApplicantAsync(IdentityDocumentsCheckInputData checkData,
                                                                CancellationToken cancellationToken)
        {
            var applicant = new Applicant
            {
                FirstName = checkData.FullName.FirstName,
                LastName = checkData.FullName.LastName,
                Dob = checkData.BirthDate
            };
            return UpdateApplicantAsync(checkData.ApplicantId, applicant, cancellationToken);
        }

        protected override Task<Check> CreateCheckAsync(IdentityDocumentsCheckInputData checkData,
                                                        CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_configuration.ReportName))
                throw new CheckExecutionException(ErrorCodes.ConfigurationError, "Report name is not configured for check.");

            var targetDocuments = new[] { checkData.IdentityDocument };
            var checkRequest = new CheckRequest
            {
                ApplicantId = checkData.ApplicantId,
                DocumentIds = targetDocuments.ExtractFileExternalId(),
                ReportNames = new List<string> { _configuration.ReportName },
                Asynchronous = true,
                ApplicantProvidesData = false
            };
            return CreateCheckAsync(checkRequest, cancellationToken);
        }

        protected override Task<CheckProcessingResult> GetResultAsync(Check check, ReportList reports) =>
            _checkResultProcessor.ProcessAsync(check, reports.Reports);
    }
}