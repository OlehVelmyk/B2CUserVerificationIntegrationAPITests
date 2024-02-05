using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using TaskExtensions = WX.B2C.User.Verification.Extensions.TaskExtensions;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/lookup/countries")]
    public class LookupController : ApiController
    {
        private readonly IOptionsProvider _optionsProvider;
        private readonly ICountryMapper _countryMapper;

        public LookupController(IOptionsProvider optionsProvider, ICountryMapper countryMapper)
        {
            _optionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
            _countryMapper = countryMapper ?? throw new ArgumentNullException(nameof(countryMapper));
        }

        [HttpGet("")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CountryDto[]>> GetCountriesAsync()
        {
            var (countries, regions, supportedCountries, supportedStates, phoneCodes)
                = await TaskExtensions.WhenAll(
                    _optionsProvider.GetAsync<CountriesOption>().Select(option => option.Countries),
                    _optionsProvider.GetAsync<RegionsOption>().Select(option => option.Regions),
                    _optionsProvider.GetAsync<SupportedCountriesOption>().Select(option => option.Countries),
                    _optionsProvider.GetAsync<SupportedStatesOption>().Select(option => option.CountrySupportedStates),
                    _optionsProvider.GetAsync<PhoneCodesOption>().Select(option => option.CountryPhoneCodes));

            return Ok(countries.Select(Map).ToArray());

            CountryDto Map(CountryOption country) => _countryMapper.Map(country, regions, supportedCountries, supportedStates, phoneCodes);
        }
    }
}
