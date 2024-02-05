using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using BridgerReference;
using FluentAssertions;
using FsCheck;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.Common;
using WX.B2C.User.Verification.Infrastructure.Common.IoC;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.LexisNexis;
using WX.Core.TypeExtensions;
using SearchRequest = WX.B2C.User.Verification.Integration.Tests.Models.SearchRequest;

namespace WX.B2C.User.Verification.Integration.Tests.GatewayTests
{
    using static Constants;
    using static Constants.LexisNexis.BridgerConfiguration;

    internal sealed class LexisNexisBridgerClientTests : BaseIntegrationTest
    {
        private IBridgerApiClientFactory _bridgerApiClientFactory;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            RegisterKeyVault<ILexisNexisBridgerKeyVault>(containerBuilder);
            containerBuilder.Register(context =>
            {
                var bridgerConfiguration = Resolve<IConfiguration>()
                                           .GetSection(LexisNexis.SectionName)
                                           .GetSection(BridgerSectionName);
                var keyVault = context.Resolve<ILexisNexisBridgerKeyVault>();
                var appConfig = context.Resolve<IAppConfig>();

                return new BridgerApiClientSettings
                {
                    BaseUri = new Uri(appConfig.LexisNexisBridgerServiceEndpoint),
                    ClientId = keyVault.ClientId.UnSecure(),
                    RolesOrUsers = keyVault.RolesOrUsers.UnSecure(),
                    UserId = bridgerConfiguration[UserIdPath]
                };
            }).AsSelf().SingleInstance();

            containerBuilder.RegisterType<Rc2EncryptProvider>()
                            .As<IEncryptProvider>()
                            .SingleInstance();

            containerBuilder.RegisterCredentialsProviders();

            containerBuilder.Register(context =>
            {
                var bridgerConfiguration = context.Resolve<IConfiguration>()
                                                  .GetSection(LexisNexis.SectionName)
                                                  .GetSection(BridgerSectionName);

                var provider = Substitute.For<IBridgerCredentialsStorage>();
                var password = bridgerConfiguration[PasswordPath];
                provider.GetPasswordAsync(Arg.Any<string>()).ReturnsForAnyArgs(password);

                return provider;
            })
            .As<IBridgerCredentialsStorage>();

            containerBuilder.Register(_ =>
            {
                var provider = Substitute.For<IOperationContextProvider>();
                provider.GetContextOrDefault().Returns(OperationContext.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Test"));
                return provider;
            })
            .As<IOperationContextProvider>();

            containerBuilder.RegisterType<BridgerApiClientFactory>()
                            .As<IBridgerApiClientFactory>()
                            .SingleInstance();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _bridgerApiClientFactory = Resolve<IBridgerApiClientFactory>();
            Arb.Register<SearchRequestArbitrary>();
            Arb.Register<PepSearchRequestArbitrary>();
            Arb.Register<ArrayArbitrary<SearchRequest>>();
        }

        [Theory]
        public async Task ShouldReturnResponse_WhenExecuteSearch(SearchRequest request)
        {
            // Arrange
            var client = await _bridgerApiClientFactory.CreateAsync();

            // Act & Assert
            foreach (var searchName in request.SearchNames)
            {
                var response = await client.SearchAsync(request.SearchInput, searchName);
                response.Should().NotBeNull();
            }
        }

        [Theory]
        public async Task ShouldReturnPepResponse_WhenExecuteSearch(PepSearchRequest request)
        {
            // Arrange
            var client = await _bridgerApiClientFactory.CreateAsync();

            // Act & Assert
            foreach (var searchName in request.SearchNames)
            {
                var response = await client.SearchAsync(request.SearchInput, searchName);
                response.Should().NotBeNull();
                response.Records.Should().NotBeNullOrEmpty();
                Func<ResultRecord, bool> predicate = RecordPredicate;
                response.Records.Should().Match(records => records.Any(predicate));
            }

            static bool RecordPredicate(ResultRecord record) =>
                record?.Watchlist?.Matches?.Any(MatchesPredicate) ?? false;

            static bool MatchesPredicate(WLMatch match) =>
                !string.IsNullOrWhiteSpace(match.ReasonListed);
        }
        
        [Theory]
        public async Task ShouldReturnResponse_WhenExecuteSearchSeveralTimesInParallel(SearchRequest[] requests)
        {
            // Arrange
            var client = await _bridgerApiClientFactory.CreateAsync();

            // Act
            var searches = requests.SelectMany(SearchAllNames);
            var results = await searches.WhenAll();

            // Assert
            results.Should().NotContainNulls();

            IEnumerable<Task<SearchResults>> SearchAllNames(SearchRequest request) =>
                request.SearchNames.Select(searchName => client.SearchAsync(request.SearchInput, searchName));
        }
    }
}