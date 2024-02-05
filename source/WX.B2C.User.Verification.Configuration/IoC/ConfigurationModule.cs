using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using WX.B2C.User.Verification.Configuration.Models;
using WX.B2C.User.Verification.Configuration.Seed;
using WX.B2C.User.Verification.Core.Contracts.Configurations;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.Configuration.Admin;
using WX.Configuration.Contracts.Options;
using WX.Configuration.Fabric;
using WX.Configuration.IoC;
using WX.Configuration.KeyVault;
using SupportedStates = WX.B2C.User.Verification.Configuration.Seed.SupportedStates;

namespace WX.B2C.User.Verification.Configuration.IoC
{
    public static class ConfigurationModule
    {
        private const string ServiceName = "wx-b2c-user-verification";
        private const int SeedVersion = 4;

        private static readonly TimeSpan RefreshPeriod = TimeSpan.FromMinutes(5);

        public static ContainerBuilder RegisterConfiguration(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var serviceCollection = new ServiceCollection().RegisterConfiguration();
            builder.Populate(serviceCollection);

            builder.RegisterType<OptionsProvider>()
                   .As<IOptionProvider>()
                   //For using option provider explicitly
                   .As<IOptionProvider<SupportedStatesOption>>()
                   .As<IOptionProvider<SupportedCountriesOption>>()
                   .As<IOptionProvider<RegionsOption>>()
                   .As<IOptionProvider<ActionsOption>>()
                   .As<IOptionProvider<BlacklistedCountriesOption>>()
                   .As<IOptionProvider<TicketsOption>>()
                   .As<IOptionProvider<TicketParametersMappingOption>>()
                   .As<IOptionProvider<LogLevelOption>>()
                   .As<IOptionProvider<UserReminderOption>>()
                   .SingleInstance();

            builder.RegisterType<ConfigurationManager>()
                   .As<IConfigurationManager>()
                   .SingleInstance();
            
            builder.RegisterType<OptionsMapper>()
                   .As<IOptionsMapper>()
                   .SingleInstance();

            return builder;
        }

        private static IServiceCollection RegisterConfiguration(this IServiceCollection builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterConfiguration<ApplicationConfiguration, IApplicationConfiguration>(
            x => InitOptions(x.WithServiceFabricSource()));

            return builder;
        }

        private static ConfigurationOptions InitOptions(ConfigurationOptions options) =>
            options.WithKeyVault<IMessagingKeyVault>()
                   .WithKeyVault<IB2CUserVerificationKeyVault>()
                   .WithKeyVault<IConfigurationKeyVault>()
                   .WithKeyVault<IFreshdeskKeyVault>()
                   .WithKeyVault<IPassFortKeyVault>()
                   .WithKeyVault<ILexisNexisRdpKeyVault>()
                   .WithKeyVault<ILexisNexisBridgerKeyVault>()
                   .WithKeyVault<IUserVerificationKeyVault>()
                   .WithAdminConfiguration()
                   .WithMainTableStorage<IConfigurationKeyVault>(keyVault => new StorageConnectionOptions
                   {
                       ConnectionString = keyVault.ConfigurationStorageConnectionString
                   })
                   .WithBackupBlobStorage<IConfigurationKeyVault>(keyVault => new StorageConnectionOptions
                   {
                       ConnectionString = keyVault.ConfigurationBackupStorageConnectionString
                   })
                   .WithRefresh(RefreshPeriod)
                   // TODO WRXB-**** setup call logger by wx.configuration + adapt wx.configuration to provide service provider
                   // .WithSerilogLogger<ApplicationConfiguration, IApplicationConfiguration, IB2CUserVerificationKeyVault>(ServiceName,
                   //     y => y.SplunkEndpoint.UnSecure(),
                   //     y => y.SplunkToken.UnSecure(),
                   //     configuration => configuration.Enrich.With<OperationContextEnricher>())
                   // TODO WRXB-11105 Add metrics later if needed
                   // .WithMetricsLogger<IApplicationConfiguration, IMetricsLogger>((config, provider) =>
                   //                                                                   new PrometheusMetricsLogger(config, new PrometheusMetricsFactory(config)))
                   .SelectServiceNamespace(ServiceName);

        public static ContainerBuilder RegisterConfigurationSeeds(this ContainerBuilder builder)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterConfigurationSeed<ApplicationConfiguration, IApplicationConfiguration>(x =>
                {
                    x.Service = new ServiceConfiguration
                    {
                        Version = SeedVersion,
                        Actions = Actions.Seed,
                        HostsLogLevel = Array.Empty<HostLogLevel>(),
                        Countries = new CountriesConfiguration
                        {
                            Regions = Regions.Seed,
                            BlacklistedCountries = BlacklistedCountries.Seed,
                            SupportedCountries = SupportedCountries.Seed,
                            SupportedStates = SupportedStates.Seed
                        },
                        Tickets = new TicketsConfiguration
                        {
                            Tickets = Tickets.Seed,
                            TicketParametersMapping = TicketsParametersMapping.Seed
                        },
                        ReminderSpans = ReminderSpans.Seed(x.Environment)
                    };
                    return x;
                },
                x => x.Service == null || x.Service.Version < SeedVersion);

            builder.Populate(serviceCollection);
            return builder;
        }
    }
}
