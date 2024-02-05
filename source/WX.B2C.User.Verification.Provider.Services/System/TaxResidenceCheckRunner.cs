using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class TaxResidenceCheckRunner : SyncCheckRunner<TaxResidenceInputData>
    {
        private readonly string[] _countries;

        public TaxResidenceCheckRunner(string[] countries)
        {
            _countries = countries ?? throw new ArgumentNullException(nameof(countries));
        }

        protected override Task<CheckProcessingResult> RunSync(TaxResidenceInputData checkData)
        {
            var matches = _countries.Intersect(checkData.TaxResidence);
            var outputData = new TaxResidenceOutputData
            {
                MatchedCountries = matches.ToArray()
            };

            var checkProcessingResult = !outputData.MatchedCountries.Any()
                ? CheckProcessingResult.Passed(outputData)
                : CheckProcessingResult.Failed(outputData);

            return Task.FromResult(checkProcessingResult);
        }
    }
}