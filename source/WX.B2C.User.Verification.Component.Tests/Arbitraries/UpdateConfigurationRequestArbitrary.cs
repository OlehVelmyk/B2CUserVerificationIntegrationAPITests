using System.Collections.Generic;
using System.Linq;
using FsCheck;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class UpdateConfigurationRequestArbitrary : Arbitrary<UpdateConfigurationRequest>
    {
        public static Arbitrary<UpdateConfigurationRequest> Create() => new UpdateConfigurationRequestArbitrary();

        public override Gen<UpdateConfigurationRequest> Generator =>
            from blacklistCountries in CountryCodeGenerators.Supported().ArrayOf().OrNull()
            from supportedCountries in CountryCodeGenerators.Supported().ArrayOf().OrNull()
            from supportedStates in SupportedStatesGenerator.ArrayOf().OrNull()
            from regions in RegionGenerator.ArrayOf().OrNull()
            from regionActions in RegionActionsGenerator.ArrayOf().OrNull()
            from parameters in TicketConfigurationGenerators.Parameters.OrNull()
            from tickets in TicketConfigurationGenerators.Ticket(parameters).ArrayOf().OrNull()
            from parametersMapping in TicketConfigurationGenerators.ParametersMapping(parameters).OrNull()
            from hostLogLevels in HostLogLevelGenerator.ArrayOf().OrNull()
            select new UpdateConfigurationRequest
            {
                BlacklistedCountries = blacklistCountries,
                SupportedCountries = supportedCountries,
                SupportedStates = supportedStates,
                Regions = regions,
                RegionActions = regionActions,
                Tickets = tickets,
                TicketParametersMapping = parametersMapping,
                HostLogLevels = hostLogLevels
            };

        private static Gen<SupportedStatesDto> SupportedStatesGenerator =>
            from country in CountryCodeGenerators.Supported()
            from states in StateGenerators.Us().ArrayOf()
            where states.Length > 1
            select new SupportedStatesDto
            {
                Country = country,
                States = states.Distinct().ToList()
            };

        private static Gen<RegionDto> RegionGenerator =>
            from region in Gen.Elements("EEA", "RoW", "APAC", "Global")
            from countries in CountryCodeGenerators.Supported().ArrayOf()
            where countries.Length > 1
            select new RegionDto
            {
                Name = region,
                Countries = countries.Distinct().ToList()
            };

        private static Gen<RegionActionsDto> RegionActionsGenerator =>
            from regionType in Arb.Generate<RegionType>()
            from regionName in StringGenerators.LettersOnly(3, 10)
            from actions in ActionGenerator.ArrayOf()
            where actions.Length > 1
            select new RegionActionsDto
            {
                RegionType = regionType,
                Region = regionName,
                Actions = actions
            };

        private static Gen<ActionDto> ActionGenerator =>
            from actionType in Arb.Generate<ActionType>()
            from xpath in StringGenerators.LettersOnly(10, 20)
            from priority in Gen.Choose(1, 12)
            from metadata in Arb.Generate<Dictionary<string, string>>().OrNull()
            select new ActionDto
            {
                ActionType = actionType,
                XPath = xpath,
                Priority = priority,
                Metadata = metadata
            };

        private static Gen<HostLogLevelDto> HostLogLevelGenerator =>
            from hostName in StringGenerators.LettersOnly(3, 10)
            from logLevel in Arb.Generate<LogEventLevel>()
            select new HostLogLevelDto
            {
                Host = hostName,
                Level = logLevel
            };
    }
}
