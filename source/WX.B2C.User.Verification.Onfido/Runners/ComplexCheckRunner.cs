using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Configurations;
using WX.B2C.User.Verification.Onfido.Extensions;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Runners
{
    internal class ComplexCheckRunner : OnfidoCheckRunner<ComplexCheckInputData>
    {
        private readonly ComplexCheckConfiguration _configuration;
        private readonly ICountryDetailsProvider _countryDetailsProvider;

        public ComplexCheckRunner(
            ComplexCheckConfiguration configuration,

            IOnfidoApiClientFactory clientFactory,
            ICountryDetailsProvider countryDetailsProvider)
            : base(clientFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
        }

        protected override async Task<Applicant> UpdateApplicantAsync(ComplexCheckInputData checkData,
                                                                      CancellationToken cancellationToken)
        {
            var applicant = new Applicant();

            if (checkData.BirthDate.HasValue)
                applicant.Dob = checkData.BirthDate.Value;

            if (checkData.FullName is { } fullName)
            {
                applicant.FirstName = fullName.FirstName;
                applicant.LastName = fullName.LastName;
            }

            if (checkData.Address is { } residenceAddress)
            {
                var countryAlpha3Code = await _countryDetailsProvider.FindAlpha3Async(residenceAddress.Country);
                applicant.Address = new ApplicantAddress
                {
                    Country = countryAlpha3Code,
                    State = residenceAddress.State,
                    Postcode = residenceAddress.ZipCode,
                    Line1 = residenceAddress.Line1
                };
            }

            return await UpdateApplicantAsync(checkData.ApplicantId, applicant, cancellationToken);
        }

        protected override Task<Check> CreateCheckAsync(ComplexCheckInputData checkData,
                                                        CancellationToken cancellationToken)
        {
            var reportNames = _configuration.Configurations
                                            .Cast<OnfidoCheckConfiguration>()
                                            .Select(x => x.ReportName)
                                            .Distinct()
                                            .ToList();

            var targetDocuments = new List<DocumentDto>();
            if (checkData.IdentityDocument != null)
                targetDocuments.Add(checkData.IdentityDocument);
            if (checkData.Selfie != null)
                targetDocuments.Add(checkData.Selfie);

            var checkRequest = new CheckRequest
            {
                ApplicantId = checkData.ApplicantId,
                DocumentIds = targetDocuments.ExtractFileExternalId(),
                ReportNames = reportNames,
                Asynchronous = true,
                ApplicantProvidesData = false
            };
            return CreateCheckAsync(checkRequest, cancellationToken);
        }

        protected override Task<CheckProcessingResult> GetResultAsync(Check check, ReportList reports) => throw new NotSupportedException();
    }
}
