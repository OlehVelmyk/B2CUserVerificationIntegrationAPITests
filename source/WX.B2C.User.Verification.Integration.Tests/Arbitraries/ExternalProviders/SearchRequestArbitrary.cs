using System;
using BridgerReference;
using FsCheck;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Integration.Tests.Models;
using SearchRequest = WX.B2C.User.Verification.Integration.Tests.Models.SearchRequest;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders
{
    using static Integration.Tests.Constants.LexisNexis;

    internal class SearchRequestArbitrary : Arbitrary<SearchRequest>
    {
        private const string Locale = "en_US";
        private const string Country = "US";

        public static Arbitrary<SearchRequest> Create() => new SearchRequestArbitrary();

        public override Gen<SearchRequest> Generator =>
            from seed in Arb.Generate<int>()
            from firstName in PersonalDetailsGenerator.FirstName(seed, Locale)
            from lastName in PersonalDetailsGenerator.LastName(seed, Locale)
            from dob in DateTimeGenerators.BeforeUtcNow(-18)
            from ssnNumber in StringGenerators.Numbers(9)
            from address_ in AddressGenerator.Address(seed, Locale)
            from email in PersonalDetailsGenerator.Email(seed, Locale).OrNull()
            let name = new InputName
            {
                First = firstName,
                Last = lastName
            }
            let address = new InputAddress()
            {
                Type = AddressType.Current,
                Country = Country,
                PostalCode = address_.ZipCode,
                City = address_.City,
                StateProvinceDistrict = address_.State,
                Street1 = address_.Line1,
                Street2 = address_.Line2
            }
            let dobInfo = new InputAdditionalInfo()
            {
                Type = AdditionalInfoType.DOB,
                Date = new Date
                {
                    Year = dob.Year,
                    Month = dob.Month,
                    Day = dob.Day
                }
            }
            let emailInfo = new InputAdditionalInfo
            {
                Type = AdditionalInfoType.Other,
                Label = "Email",
                Value = email
            }
            let additionalInfo = email is null
                ? new[] { dobInfo }
                : new[] { dobInfo, emailInfo }
            let tin = new InputID
            {
                Type = IDType.SSN,
                Number = ssnNumber
            }
            let inputRecord = new InputRecord()
            {
                Entity = new InputEntity
                {
                    EntityType = InputEntityType.Individual,
                    Gender = GenderType.Unknown,
                    Name = name,
                    AdditionalInfo = additionalInfo,
                    Addresses = new[] { address },
                    IDs = new[] { tin }
                }
            }
            let input = new SearchInput
            {
                Records = new[] { inputRecord }
            }
            select new SearchRequest
            {
                SearchNames = GetSearchNames(),
                SearchInput = input
            };

        private string[] GetSearchNames() =>
            new[]
            {
                BridgerSearchModes.Pep,
                BridgerSearchModes.Sanction,
                BridgerSearchModes.AdverseMedia
            };
    }

    internal class PepSearchRequestArbitrary : Arbitrary<PepSearchRequest>
    {
        private const string Country = "US";

        public static Arbitrary<PepSearchRequest> Create() => new PepSearchRequestArbitrary();

        public override Gen<PepSearchRequest> Generator =>
            from seed in Arb.Generate<int>()
            let dob = DateTime.Parse(HardcodedPersonalData.BridgerPepResultDob)
            let name = new InputName
            {
                First = HardcodedPersonalData.BridgerPepResultFirstName,
                Last = HardcodedPersonalData.BridgerPepResultLastName
            }
            let address = new InputAddress()
            {
                Type = AddressType.Current,
                Country = Country,
                PostalCode = HardcodedPersonalData.BridgerPepResultZipCode,
                City = HardcodedPersonalData.BridgerPepResultCity,
                StateProvinceDistrict = HardcodedPersonalData.BridgerPepResultState,
                Street1 = HardcodedPersonalData.BridgerPepResultAddressLine
            }
            let additionalInfo = new InputAdditionalInfo()
            {
                Type = AdditionalInfoType.DOB,
                Date = new Date
                {
                    Year = dob.Year,
                    Month = dob.Month,
                    Day = dob.Day
                }
            }
            let inputRecord = new InputRecord()
            {
                Entity = new InputEntity
                {
                    EntityType = InputEntityType.Individual,
                    Gender = GenderType.Unknown,
                    Name = name,
                    AdditionalInfo = new[] { additionalInfo },
                    Addresses = new[] { address }
                }
            }
            let input = new SearchInput
            {
                Records = new[] { inputRecord }
            }
            select new PepSearchRequest
            {
                SearchNames = new[] { BridgerSearchModes.Pep },
                SearchInput = input
            };
    }
}
