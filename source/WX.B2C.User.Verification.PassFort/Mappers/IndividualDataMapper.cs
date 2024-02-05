using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.PassFort.Client.Models;

namespace WX.B2C.User.Verification.PassFort.Mappers
{
    internal interface IIndividualDataMapper
    {
        Task<FreeformAddress> MapAsync(AddressDto address);

        string MapBirthDate(DateTime birthdate);

        Task<string> MapNationalityAsync(string alpha2);

        FullName MapFullName(Core.Contracts.Dtos.FullNameDto fullName);

        Task<DocumentMetadataIdentityNumber> Map(string issuingCountry, IdDocumentNumberDto idDocumentNumber);
    }

    internal sealed class IndividualDataMapper : IIndividualDataMapper
    {
        private readonly ICountryDetailsProvider _countryDetailsProvider;
        private readonly IPassFortEnumerationsMapper _enumerationsMapper;
        private static readonly IDictionary<string, string> PassFortCountryMap = new Dictionary<string, string>()
        {
            ["XKX"] = "XXX"
        };

        public IndividualDataMapper(ICountryDetailsProvider countryDetailsProvider, IPassFortEnumerationsMapper enumerationsMapper)
        {
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
            _enumerationsMapper = enumerationsMapper ?? throw new ArgumentNullException(nameof(enumerationsMapper));
        }

        public Task<FreeformAddress> MapAsync(AddressDto address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            // TODO: Should we support structured address?
            ////if (!IsFreeformAddress(address))
            ////    return ToStructuredAddress(address);

            return ToFreeFormAddressAsync(address);
        }

        public string MapBirthDate(DateTime birthdate) => birthdate.ToString("yyyy-MM-dd");

        public Task<string> MapNationalityAsync(string alpha2) => MapCountryCode(alpha2);

        public FullName MapFullName(Core.Contracts.Dtos.FullNameDto fullName)
        {
            if (fullName == null)
                throw new ArgumentNullException(nameof(fullName));

            return new FullName
            {
                FamilyName = fullName.LastName,
                GivenNames = new List<string> { fullName.FirstName }
            };
        }

        public async Task<DocumentMetadataIdentityNumber> Map(string issuingCountry, IdDocumentNumberDto idDocumentNumber)
        {
            if (issuingCountry is null || idDocumentNumber is null)
                return null;

            var countryCode = await MapCountryCode(issuingCountry);
            if (countryCode is null)
                return null;

            var documentType = _enumerationsMapper.MapIdentityType(idDocumentNumber.Type);
            return new DocumentMetadataIdentityNumber
            {
                CountryCode = countryCode,
                Number = idDocumentNumber.Number,
                DocumentType = documentType
            };
        }

        private async Task<FreeformAddress> ToFreeFormAddressAsync(AddressDto address)
        {
            var text = string.IsNullOrEmpty(address.Line2)
                ? string.Join(", ", address.Line1, address.City, address.State, address.ZipCode)
                : string.Join(", ", address.Line2, address.Line1, address.City, address.State, address.ZipCode);

            var alpha3 = await MapCountryCode(address.Country);

            return new FreeformAddress
            {
                Country = alpha3,
                Text = text
            };
        }

        private async Task<string> MapCountryCode(string alpha2)
        {
            var alpha3 = await _countryDetailsProvider.FindAlpha3Async(alpha2);
            return alpha3 is not null && PassFortCountryMap.TryGetValue(alpha3, out var result) ? result : alpha3;
        }

        ////private static StructuredAddress ToStructuredAddress(ResidenceAddress address)
        ////{
        ////    var countryAlpha3Code = address.Country; //TODO: convert to alpha2 code

        ////    var hasCounty = HasCounty(countryAlpha3Code);
        ////    var hasStateProvince = HasStateProvince(countryAlpha3Code);

        ////    var stateProvince = hasStateProvince ? address.State : null;
        ////    var county = hasCounty ? address.State : null;

        ////    return new StructuredAddress
        ////    {
        ////        Country = countryAlpha3Code,
        ////        Locality = address.City,
        ////        PostalCode = address.ZipCode,
        ////        Route = address.Street,
        ////        StreetNumber = address.StreetNumber,
        ////        Subpremise = address.Flat,
        ////        StateProvince = stateProvince,
        ////        County = county
        ////    };
        ////}

        ////private static bool IsFreeformAddress(ResidenceAddress address)
        ////{
        ////    return string.IsNullOrEmpty(address.Street) ||
        ////           string.IsNullOrEmpty(address.StreetNumber);
        ////}

        ////private static bool HasCounty(string countryAlpha3Code)
        ////{
        ////    return countryAlpha3Code == "GBR";
        ////}

        ////private static bool HasStateProvince(string countryAlpha3Code)
        ////{
        ////    return countryAlpha3Code != "GBR";
        ////}
    }
}