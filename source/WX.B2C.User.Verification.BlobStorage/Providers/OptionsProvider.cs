using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.BlobStorage.Dto;
using WX.B2C.User.Verification.BlobStorage.Mappers;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;

namespace WX.B2C.User.Verification.BlobStorage.Providers
{
    internal class OptionsProvider : IOptionProvider<CountriesOption>, 
                                     IOptionProvider<PhoneCodesOption>,
                                     IOptionProvider<ExcludedNamesOption>
    {
        private readonly IConfigurationBlobStorage _blobStorage;
        private readonly IOptionMapper _mapper;

        public OptionsProvider(IConfigurationBlobStorage blobStorage, IOptionMapper mapper)
        {
            _blobStorage = blobStorage ?? throw new ArgumentNullException(nameof(blobStorage));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CountriesOption> GetAsync()
        {
            var countriesDto = await _blobStorage.GetAsync<CountriesDto>();
            return _mapper.Map(countriesDto);
        }

        async Task<PhoneCodesOption> IOptionProvider<PhoneCodesOption>.GetAsync()
        {
            var phoneCodesDto = await _blobStorage.GetAsync<PhoneCodeDto[]>();
            return _mapper.Map(phoneCodesDto);
        }

        async Task<ExcludedNamesOption> IOptionProvider<ExcludedNamesOption>.GetAsync()
        {
            var excludedNamesDto = await _blobStorage.GetAsync<ExcludedNameDto[]>();
            return _mapper.Map(excludedNamesDto);
        }
    }
}