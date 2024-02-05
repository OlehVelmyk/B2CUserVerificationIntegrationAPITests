using System.Collections.Generic;
using System.Linq;
using BridgerReference;
using WX.B2C.User.Verification.LexisNexis.Models;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.LexisNexis.Processors
{
    internal sealed class RiskScreeningCheckProcessor
    {
        public CheckProcessingResult Process(IDictionary<string, SearchResults> searchResults)
        {
            // TODO: Set raw data
            var outputData = new LexisNexisRiskScreeningOutputData { };
            var hasAnyMatches = false;

            foreach (var (mode, searchResult) in searchResults)
            {
                var hasMatches = HasMatches(searchResult);
                hasAnyMatches |= outputData.WithMatches(mode, hasMatches);
            }

            return hasAnyMatches
                ? CheckProcessingResult.Failed(outputData)
                : CheckProcessingResult.Passed(outputData);
        }

        private static bool HasMatches(SearchResults results)
        {
            return results.Records != null
                   && results.Records.Any(x =>
                       x?.Watchlist?.Matches != null
                       && x.Watchlist.Matches.Any(match => !string.IsNullOrEmpty(match.ReasonListed)));
        }
    }
}
