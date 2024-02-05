using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Extensions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using ErrorCodes = WX.B2C.User.Verification.Provider.Contracts.Constants.ErrorCodes;

namespace WX.B2C.User.Verification.Onfido.Runners
{
    internal abstract class OnfidoCheckRunner<TData> : AsyncCheckRunner<TData> where TData : OnfidoCheckInputData
    {
        private readonly IOnfidoApiClientFactory _clientFactory;

        protected OnfidoCheckRunner(IOnfidoApiClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public override async Task<CheckRunningResult> RunAsync(TData checkData,
                                                                CancellationToken cancellationToken = default)
        {
            try
            {
                var applicant = await UpdateApplicantAsync(checkData, cancellationToken);
                var check = await CreateCheckAsync(checkData, cancellationToken);

                return CheckRunningResult.Instructed(check.Id);
            }
            catch (ValidationException exc)
            {
                throw new CheckExecutionException(ErrorCodes.InvalidInputData, exc.Message);
            }
            catch (OnfidoApiErrorException exc)
            {
                var error = exc.Body?.Error;
                var additionalData = new Dictionary<string, object>
                {
                    [nameof(error.Message)] = error?.Message,
                    [nameof(error.Fields)] = error?.Fields?.ToString()
                };
                throw new CheckExecutionException(ErrorCodes.ProviderUnknownError, error?.Type, additionalData);
            }
        }

        public override async Task<CheckProcessingResult> GetResultAsync(CheckProcessingContext context,
                                                                         CancellationToken cancellationToken = default)
        {
            try
            {
                var checkId = context.ExternalData.GetExternalId();

                using var client = _clientFactory.Create();

                var check = await client.Checks.FindAsync(checkId, cancellationToken);
                var reports = await client.Reports.ListAsync(checkId, cancellationToken);

                return await GetResultAsync(check, reports);
            }
            catch (ValidationException exc)
            {
                throw new CheckProcessingException(ErrorCodes.InvalidInputData, exc.Message);
            }
            catch (OnfidoApiErrorException exc)
            {
                var error = exc.Body?.Error;
                var additionalData = new Dictionary<string, object>
                {
                    [nameof(error.Message)] = error?.Message, 
                    [nameof(error.Fields)] = error?.Fields
                };
                throw new CheckProcessingException(ErrorCodes.ProviderUnknownError, error?.Type, additionalData);
            }
        }

        protected abstract Task<Applicant> UpdateApplicantAsync(TData checkData, CancellationToken cancellationToken);

        protected abstract Task<Check> CreateCheckAsync(TData checkData, CancellationToken cancellationToken);

        protected abstract Task<CheckProcessingResult> GetResultAsync(Check check, ReportList reports);

        protected async Task<Applicant> UpdateApplicantAsync(string applicantId, Applicant applicant,
                                                             CancellationToken cancellationToken)
        {
            using var client = _clientFactory.Create();
            return await client.Applicants.UpdateAsync(applicant, applicantId, cancellationToken);
        }

        protected async Task<Check> CreateCheckAsync(CheckRequest checkRequest,
                                                     CancellationToken cancellationToken)
        {
            using var client = _clientFactory.Create();
            return await client.Checks.CreateAsync(checkRequest, cancellationToken);
        }
    }
}
