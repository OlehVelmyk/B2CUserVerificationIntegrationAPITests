using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
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
    internal sealed class FacialSimilarityCheckRunner : OnfidoCheckRunner<FacialSimilarityCheckInputData>
    {
        private readonly FacialSimilarityCheckConfiguration _configuration;
        private readonly IFacialSimilarityCheckResultProcessor _checkResultProcessor;

        public FacialSimilarityCheckRunner(
            FacialSimilarityCheckConfiguration configuration,
            IOnfidoApiClientFactory clientFactory,
            IFacialSimilarityCheckResultProcessor checkResultProcessor) : base(clientFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _checkResultProcessor = checkResultProcessor ?? throw new ArgumentNullException(nameof(checkResultProcessor));
        }

        protected override Task<Applicant> UpdateApplicantAsync(FacialSimilarityCheckInputData checkData,
                                                                CancellationToken cancellationToken)
        {
            var applicant = new Applicant
            {
                FirstName = checkData.FullName.FirstName,
                LastName = checkData.FullName.LastName
            };
            return UpdateApplicantAsync(checkData.ApplicantId, applicant, cancellationToken);
        }

        protected override Task<Check> CreateCheckAsync(FacialSimilarityCheckInputData checkData,
                                                        CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_configuration.ReportName))
                throw new CheckExecutionException(ErrorCodes.ConfigurationError, "Report name is not configured for check.");

            var targetDocuments = new List<DocumentDto> { checkData.IdentityDocument  };
            if (checkData.Selfie != null || !_configuration.IsSelfieOptional)
                targetDocuments.Add(checkData.Selfie);
            
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
