using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using NUnit.Framework;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.Configuration.IoC;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Triggers.Configs;
using WX.B2C.User.Verification.Core.Services.Configuration;
using WX.B2C.User.Verification.Core.Services.Tickets;
using WX.B2C.User.Verification.DataAccess.Seed;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Integration.Tests.Configuration
{
    //FIX registration to read not from fabric
    // [TestFixture]
    // public class ConfigurationTests : BaseIntegrationTest
    // {
    //     protected override void RegisterModules(ContainerBuilder containerBuilder)
    //     {
    //         containerBuilder.RegisterConfiguration();
    //         containerBuilder.RegisterBlobStorage();
    //         containerBuilder.RegisterType<OptionsProvider>().As<IOptionsProvider>();
    //         containerBuilder.RegisterType<DefaultSystemClock>().As<ISystemClock>();
    //     }
    //
    //     [Test]
    //     public async Task ShouldReadRegionsFromConfig()
    //     {
    //         var configProvider = Resolve<IOptionsProvider>();
    //
    //         var option = await configProvider.GetAsync<RegionsOption>();
    //
    //         option.Should().NotBeNull();
    //         option.Regions.Should().NotBeNullOrEmpty();
    //         option.Regions.Should().Contain(option => option.Name == "APAC");
    //     }
    //
    //     [Test]
    //     public async Task ShouldFindRegion_ForAllSupportedCountries()
    //     {
    //         var configProvider = Resolve<IOptionsProvider>();
    //         var supportedCountriesOption = await configProvider.GetAsync<SupportedCountriesOption>();
    //
    //         var option = await configProvider.GetAsync<RegionsOption>();
    //
    //         using (new AssertionScope())
    //         {
    //             foreach (var country in supportedCountriesOption.Countries)
    //             {
    //                 var regions = option.Regions.Where(regionOption => regionOption.Countries.Contains(country)).ToArray();
    //                 regions.Should().NotBeEmpty($"Country {country} is not found in any regions");
    //                 regions.Should().HaveCountLessOrEqualTo(1, $"Country {country} is found in more than one region");
    //             }
    //         }
    //     }
    //
    //     [Test]
    //     public async Task ShouldReadSupportedStatesFromConfig()
    //     {
    //         var configProvider = Resolve<IOptionsProvider>();
    //
    //         var option = await configProvider.GetAsync<SupportedStatesOption>();
    //
    //         option.Should().NotBeNull();
    //         option.CountrySupportedStates.Should().ContainKey("US").WhoseValue.Should().NotBeNullOrEmpty();
    //     }
    //
    //     [Test]
    //     public async Task ShouldReadSupportedCountriesFromConfig()
    //     {
    //         var configProvider = Resolve<IOptionsProvider>();
    //
    //         var option = await configProvider.GetAsync<SupportedCountriesOption>();
    //
    //         option.Should().NotBeNull();
    //         option.Countries.Should().NotBeNullOrEmpty();
    //     }
    //
    //     [Test]
    //     public async Task ShouldReadCountriesFromConfig()
    //     {
    //         var configProvider = Resolve<IOptionsProvider>();
    //
    //         var option = await configProvider.GetAsync<CountriesOption>();
    //
    //         option.Should().NotBeNull();
    //         option.Countries.Should().NotBeNullOrEmpty();
    //         option.Countries.Single(countryOption => countryOption.Alpha2Code == "US").States.Should().NotBeNullOrEmpty();
    //         option.Countries.Should().NotContainNulls(country => country.Alpha2Code);
    //         option.Countries.Should().NotContainNulls(country => country.Alpha3Code);
    //         option.Countries.Should().NotContainNulls(country => country.Name);
    //         option.Countries.Should().OnlyHaveUniqueItems(country => country.Alpha2Code);
    //         option.Countries.Should().OnlyHaveUniqueItems(country => country.Alpha3Code);
    //         option.Countries.Should().OnlyHaveUniqueItems(country => country.Name);
    //     }
    //
    //     [Test]
    //     public async Task ShouldReadActionsFromConfig()
    //     {
    //         var configProvider = Resolve<IOptionsProvider>();
    //
    //         var option = await configProvider.GetAsync<ActionsOption>();
    //
    //         option.Should().NotBeNull();
    //         option.RegionActions.Should().NotBeNullOrEmpty();
    //         var actions = option.RegionActions.SelectMany(actionsOption => actionsOption.Actions);
    //
    //         var surveyActions = actions.Where(actionOption => actionOption.ActionType == ActionType.Survey).ToArray();
    //         surveyActions.Foreach(actionOption => actionOption.Metadata.ContainsKey("SurveyId"));
    //         surveyActions.Foreach(actionOption => actionOption.XPath.Contains(actionOption.Metadata["SurveyId"]));
    //     }
    //
    //     [Test]
    //     public async Task ShouldReadBlacklistedCountriesFromConfig()
    //     {
    //         var configProvider = Resolve<IOptionsProvider>();
    //
    //         var option = await configProvider.GetAsync<BlacklistedCountriesOption>();
    //
    //         option.Should().NotBeNull();
    //         option.Countries.Should().NotBeNullOrEmpty();
    //         option.Countries.Should().Contain("CI");
    //     }
    //
    //     [Test]
    //     public async Task ShouldReadPhoneCodesFromConfig()
    //     {
    //         var configProvider = Resolve<IOptionsProvider>();
    //
    //         var option = await configProvider.GetAsync<PhoneCodesOption>();
    //
    //         option.Should().NotBeNull();
    //         option.CountryPhoneCodes.Should().NotBeNullOrEmpty();
    //         option.CountryPhoneCodes.Should().ContainKey("UA").WhoseValue.Should().Be("380");
    //     }
    //
    //     [Test]
    //     public async Task ShouldReadTicketConfigurations()
    //     {
    //         var hardcodedReasons = typeof(TicketReasons)
    //                                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
    //                                .Select(info => info.GetValue(null).ToString());
    //         
    //         var policyTriggers = SeedData.Triggers.SelectMany(variant => variant.Commands)
    //                                      .Where(command => command.Type == CommandType.SendTicket)
    //                                      .Select(command => JsonConvert.DeserializeObject<SendTickedCommandConfig>(command.Config))
    //                                      .Select(config => config.Reason);
    //         var knownReasons = hardcodedReasons.Concat(policyTriggers).Distinct().ToArray();
    //         
    //         var configProvider = Resolve<IOptionsProvider>();
    //
    //         var option = await configProvider.GetAsync<TicketsOption>();
    //
    //         option.Should().NotBeNull();
    //         option.Get("repeating-turnover-threshold-reached")
    //               .Should()
    //               .NotBeNull()
    //               .And.BeOfType<TicketConfig>()
    //               .Which.Formats.Should()
    //               .NotBeNullOrEmpty();
    //         
    //         option.Get("additional-docs-review-needed")
    //               .Should()
    //               .NotBeNull()
    //               .And.BeOfType<TicketConfig>()
    //               .Which.Formats.Should()
    //               .BeEmpty();
    //
    //         using (new AssertionScope())
    //         {
    //             foreach (var reason in knownReasons)
    //             {
    //                 var act = new Func<TicketConfig>(() => option.Get(reason));
    //                 act.Should()
    //                    .NotThrow()
    //                    .Subject
    //                    .Should()
    //                    .NotBeNull()
    //                    .And.BeOfType<TicketConfig>();
    //             }
    //         }
    //     }
    //     
    //     [Test]
    //     public async Task ShouldTicketParametersMapping()
    //     {
    //         var configProvider = Resolve<IOptionsProvider>();
    //
    //         var option = await configProvider.GetAsync<TicketParametersMappingOption>();
    //
    //         option.Should().NotBeNull();
    //         option.GetMapping("IdDocumentNumber").Should().NotBeNull();
    //     }
    //
    //     [Test]
    //     public async Task ShouldReadExcludedNamesFromConfig()
    //     {
    //         var configProvider = Resolve<IOptionsProvider>();
    //
    //         var option = await configProvider.GetAsync<ExcludedNamesOption>();
    //
    //         option.Should().NotBeNull();
    //         option.Names.Should().NotBeNullOrEmpty();
    //     }
    //
    //     [Test]
    //     public async Task ShouldReadingCountriesFromConfigBeFast_WhenCalledSecondTime()
    //     {
    //         var configProvider = Resolve<IOptionsProvider>();
    //
    //         Func<Task> firstTime = () => configProvider.GetAsync<CountriesOption>();
    //         Func<Task> secondTime = () => configProvider.GetAsync<CountriesOption>();
    //
    //         await firstTime.Should().NotThrowAsync();
    //         secondTime.ExecutionTime().Should().BeLessThan(TimeSpan.FromMilliseconds(10));
    //     }
    // }
}