using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
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
    internal sealed class IdentityEnhancedCheckRunner : OnfidoCheckRunner<IdentityEnhancedCheckInputData>
    {
        private readonly IdentityEnhancedCheckConfiguration _configuration;
        private readonly ICountryDetailsProvider _countryDetailsProvider;
        private readonly IIdentityEnhancedCheckResultProcessor _checkResultProcessor;

        public IdentityEnhancedCheckRunner(
            IdentityEnhancedCheckConfiguration configuration,
            IOnfidoApiClientFactory clientFactory,
            IIdentityEnhancedCheckResultProcessor checkResultProcessor,
            ICountryDetailsProvider countryDetailsProvider) : base(clientFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
            _checkResultProcessor = checkResultProcessor ?? throw new ArgumentNullException(nameof(checkResultProcessor));
        }

        protected override async Task<Applicant> UpdateApplicantAsync(IdentityEnhancedCheckInputData checkData, 
                                                                      CancellationToken cancellationToken)
        {
            var countryAlpha3Code = await _countryDetailsProvider.FindAlpha3Async(checkData.Address.Country);
            if (string.IsNullOrEmpty(countryAlpha3Code))
                throw new CheckExecutionException(ErrorCodes.InvalidInputData, "Could not resolve country by alpha2 code.");

            var applicant = new Applicant
            {
                FirstName = checkData.FullName.FirstName,
                LastName = checkData.FullName.LastName,
                Dob = checkData.BirthDate,
                Address = new ApplicantAddress
                {
                    Country = countryAlpha3Code,
                    State = checkData.Address.State,
                    Postcode = checkData.Address.ZipCode,
                    Line1 = checkData.Address.Line1
                }
            };
            return await UpdateApplicantAsync(checkData.ApplicantId, applicant, cancellationToken);
        }

        protected override Task<Check> CreateCheckAsync(IdentityEnhancedCheckInputData checkData, 
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

        protected override Task<CheckProcessingResult> GetResultAsync(Check check, ReportList reports)
        {
            var result = _checkResultProcessor.Process(check, reports.Reports);
            return Task.FromResult(result);
        }
    }
}