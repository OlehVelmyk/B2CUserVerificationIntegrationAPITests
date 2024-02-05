using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Configurations;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Onfido.Processors;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using ErrorCodes = WX.B2C.User.Verification.Provider.Contracts.Constants.ErrorCodes;

namespace WX.B2C.User.Verification.Onfido.Runners
{
    internal sealed class FaceDuplicationCheckRunner : OnfidoCheckRunner<FaceDuplicationCheckInputData>
    {
        private readonly FaceDuplicationCheckConfiguration _configuration;
        private readonly IFaceDuplicationCheckResultProcessor _checkResultProcessor;

        public FaceDuplicationCheckRunner(
            FaceDuplicationCheckConfiguration configuration,
            IOnfidoApiClientFactory clientFactory,
            IFaceDuplicationCheckResultProcessor checkResultProcessor) 
            : base(clientFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _checkResultProcessor = checkResultProcessor ?? throw new ArgumentNullException(nameof(checkResultProcessor));
        }

        protected override Task<Applicant> UpdateApplicantAsync(FaceDuplicationCheckInputData checkData, 
                                                                CancellationToken cancellationToken)
        {
            var applicant = new Applicant
            {
                FirstName = checkData.FullName.FirstName,
                LastName = checkData.FullName.LastName
            };
            return UpdateApplicantAsync(checkData.ApplicantId, applicant, cancellationToken);
        }

        protected override Task<Check> CreateCheckAsync(FaceDuplicationCheckInputData checkData, 
                                                        CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_configuration.ReportName))
                throw new CheckExecutionException(ErrorCodes.ConfigurationError, "Report name is not configured for check.");

            var checkRequest = new CheckRequest
            {
                ApplicantId = checkData.ApplicantId,
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