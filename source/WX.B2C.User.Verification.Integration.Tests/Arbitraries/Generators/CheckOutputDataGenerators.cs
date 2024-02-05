using System;
using System.Collections.Generic;
using System.Net.Mail;
using FsCheck;
using Bogus;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using WX.B2C.User.Verification.Provider.Services.System;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators
{
    internal static class CheckOutputDataGenerators
    {
        internal static Gen<CheckOutputData> CheckOutputData =>
            Gen.OneOf(DuplicateScreeningOutput, IdNumberDuplicationOutput,
                      IpAddressCheckOutput, SurveyAnswersCheckOutput, TaxResidenceOutput);

        internal static Gen<CheckOutputData> DuplicateScreeningOutput =>
            from amount in Gen.Choose(0, 5)
            from matches in Gen.ArrayOf(amount, DuplicateMatch)
            from rawData in Arb.Generate<string>()
            let result =  new DuplicateScreeningOutputData
            {
                Matches = matches
            }
            select (CheckOutputData)result;

        internal static Gen<CheckOutputData> IdNumberDuplicationOutput =>
            from amount in Gen.Choose(0, 5)
            from matches in Gen.ArrayOf(amount, DuplicateMatch)
            from reason in Arb.Generate<IdNumberDuplicationFailReason>()
            from rawData in Arb.Generate<string>()
            let result = new IdNumberDuplicationOutputData
            {
                Matches = matches,
                Reason = reason
            }
            select (CheckOutputData)result;

        internal static Gen<CheckOutputData> IpAddressCheckOutput =>
            from seed in Arb.Generate<int>()
            from location in IpAddressLocation(seed)
            from rawData in Arb.Generate<string>()
            let result = new IpAddressCheckOutputData
            {
                ResolvedLocation = location
            }
            select (CheckOutputData)result;

        internal static Gen<CheckOutputData> SurveyAnswersCheckOutput =>
            from matches in Arb.Generate<Dictionary<string, string[]>>()
            from rawData in Arb.Generate<string>()
            let result = new SurveyAnswersCheckOutputData
            {
                Matches = matches
            }
            select (CheckOutputData)result;

        internal static Gen<CheckOutputData> TaxResidenceOutput =>
            from amount in Gen.Choose(0, 5)
            from countries in Gen.ArrayOf(amount, CountryCodeGenerators.Countries())
            from rawData in Arb.Generate<string>()
            let result = new TaxResidenceOutputData
            {
                MatchedCountries = countries
            }
            select (CheckOutputData)result;


        private static Gen<DuplicateMatch> DuplicateMatch =>
            from userId in Arb.Generate<Guid>()
            from email in Arb.Generate<MailAddress>()
            from duplicateType in Arb.Generate<DuplicateType>()
            select new DuplicateMatch
            {
                UserId = userId, 
                Email = email.Address,
                DuplicateType = duplicateType
            };

        private static Gen<IpAddressLocation> IpAddressLocation(int seed)
        {
            var addressDataSet = new Bogus.DataSets.Address
            {
                Random = new Randomizer(seed)
            };

            var location = new IpAddressLocation
            {
                ContinentName = "North America",
                ContinentCode = "0000",
                CountryName = addressDataSet.Country(),
                CountryCode = addressDataSet.CountryCode(),
                StateName = addressDataSet.State(),
                StateCode = addressDataSet.StateAbbr(),
                City = addressDataSet.City(),
                Zip = addressDataSet.ZipCode(),
                Longitude = addressDataSet.Longitude(),
                Latitude = addressDataSet.Latitude()
            };

            return Gen.Constant(location);
        }
    }
}
