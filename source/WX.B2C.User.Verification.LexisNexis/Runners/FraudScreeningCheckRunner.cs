using System;
using System.Threading.Tasks;
using Microsoft.Rest;
using WX.B2C.User.Verification.LexisNexis.Configurations;
using WX.B2C.User.Verification.LexisNexis.Mappers;
using WX.B2C.User.Verification.LexisNexis.Models;
using WX.B2C.User.Verification.LexisNexis.Processors;
using WX.B2C.User.Verification.LexisNexis.Rdp.Client;
using WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using ErrorCodes = WX.B2C.User.Verification.Provider.Contracts.Constants.ErrorCodes;

namespace WX.B2C.User.Verification.LexisNexis.Runners
{
    internal sealed class FraudScreeningCheckRunner : SyncCheckRunner<FraudScreeningCheckData>
    {
        private readonly FraudScreeningCheckConfiguration _configuration;
        private readonly IRdpApiClientFactory _clientFactory;
        private readonly FraudScreeningCheckProcessor _checkResultProcessor;
        private readonly ICheckRequestMapper _checkMapper;

        public FraudScreeningCheckRunner(
            IRdpApiClientFactory clientFactory,
            FraudScreeningCheckProcessor checkResultProcessor,
            ICheckRequestMapper checkMapper,
            FraudScreeningCheckConfiguration configuration)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _checkResultProcessor = checkResultProcessor ?? throw new ArgumentNullException(nameof(checkResultProcessor));
            _checkMapper = checkMapper ?? throw new ArgumentNullException(nameof(checkMapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override async Task<CheckProcessingResult> RunSync(FraudScreeningCheckData checkData)
        {
            try
            {
                using var client = _clientFactory.Create();
                var person = _checkMapper.Map(checkData);
                var settings = _clientFactory.GetSettings();
                var request = new InitiateWorkflowRequest
                {
                    Type = "Initiate",
                    Persons = new[] { person },
                    Settings = settings
                };
                var response = await client.Workflow.InitiateAsync(request);
                return _checkResultProcessor.Process(response);
            }
            catch (ValidationException exc)
            {
                throw new CheckExecutionException(ErrorCodes.InvalidInputData, exc.Message);
            }
            catch (HttpOperationException exc)
            {
                // TODO: 
                throw new CheckExecutionException(ErrorCodes.ProviderUnknownError, exc.Response.Content);
            }
        }
    }
}