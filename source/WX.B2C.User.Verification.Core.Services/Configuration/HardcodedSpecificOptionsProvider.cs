using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;

namespace WX.B2C.User.Verification.Core.Services.Configuration
{
    internal class HardcodedSpecificOptionsProvider : IOptionProvider<SpecificCountriesOption>
    {
        private readonly SpecificCountryOption[] _options = new[]
        {
            new SpecificCountryOption(
                new[] { "GBD", "GBN", "GBO", "GBP", "GBS" },
                "GB",
                "GBR",
                "United Kingdom")
        };

        public Task<SpecificCountriesOption> GetAsync() =>
            Task.FromResult(new SpecificCountriesOption(_options));
    }
}
