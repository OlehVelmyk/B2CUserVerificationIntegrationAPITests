using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BridgerReference;
using WX.B2C.User.Verification.LexisNexis.Bridger.Client.Exceptions;
using WX.B2C.User.Verification.LexisNexis.Configurations;
using WX.B2C.User.Verification.LexisNexis.Mappers;
using WX.B2C.User.Verification.LexisNexis.Models;
using WX.B2C.User.Verification.LexisNexis.Processors;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using ErrorCodes = WX.B2C.User.Verification.Provider.Contracts.Constants.ErrorCodes;

namespace WX.B2C.User.Verification.LexisNexis.Runners
{
    internal sealed class RiskScreeningCheckRunner : SyncCheckRunner<RiskScreeningCheckData>
    {
        private readonly RiskScreeningCheckConfiguration _configuration;
        private readonly IBridgerApiClientFactory _bridgerApiClientFactory;
        private readonly ICheckRequestMapper _checkMapper;
        private readonly RiskScreeningCheckProcessor _checkProcessor;

        public RiskScreeningCheckRunner(
            RiskScreeningCheckConfiguration configuration,
            IBridgerApiClientFactory bridgerApiClientFactory,
            RiskScreeningCheckProcessor checkProcessor,
            ICheckRequestMapper checkMapper)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _bridgerApiClientFactory = bridgerApiClientFactory ?? throw new ArgumentNullException(nameof(bridgerApiClientFactory));
            _checkMapper = checkMapper ?? throw new ArgumentNullException(nameof(checkMapper));
            _checkProcessor = checkProcessor ?? throw new ArgumentNullException(nameof(checkProcessor));
        }

        protected override async Task<CheckProcessingResult> RunSync(RiskScreeningCheckData checkData)
        {
            var searchNames = _configuration.SearchNames;
            if (searchNames == null)
                throw new CheckExecutionException(ErrorCodes.ConfigurationError, "Search names is not configured for check.");

            var responses = new Dictionary<string, SearchResults>();

            try
            {
                var bridgerClient = await _bridgerApiClientFactory.CreateAsync();

                foreach (var searchName in searchNames)
                {
                    var request = _checkMapper.Map(checkData);
                    var response = await bridgerClient.SearchAsync(request, searchName);
                    responses.Add(searchName, response);
                }

                return _checkProcessor.Process(responses);
            }
            catch (BridgerException exc)
            {
                var additionalData = new Dictionary<string, object> { [nameof(exc.Description)] = exc.Description };
                throw new CheckExecutionException(ErrorCodes.ProviderUnknownError, exc.Message, additionalData);
            }
        }
    }
}