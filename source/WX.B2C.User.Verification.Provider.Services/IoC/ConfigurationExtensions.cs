using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Serilog.Events;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.IoC;
using WX.B2C.User.Verification.Provider.Services.Sandbox;
using WX.B2C.User.Verification.Provider.Services.System;
using WX.Logging.Autofac;

namespace WX.B2C.User.Verification.Provider.Services.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterCheckProviderService(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<AutofacCheckProviderRegistry>()
                   .As<ICheckProviderRegistry>()
                   .SingleInstance();

            builder.RegisterType<JsonCheckProviderConfigurationParser>()
                   .As<ICheckProviderConfigurationParser>()
                   .SingleInstance();

            builder.RegisterType<CheckProviderService>()
                   .As<ICheckProviderService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<CheckOutputDataSerializer>()
                   .As<ICheckOutputDataSerializer>()
                   .SingleInstance();

            builder.Register(context =>
                   {
                       var providerOptions = context.Resolve<IEnumerable<CheckProviderOptions>>();
                       var configurationTypes = providerOptions
                                                .SelectMany(
                                                    options => options.Configurations,
                                                    (options, configuration) => new
                                                    {
                                                        Metadata = new CheckProviderMetadata(configuration.Key, options.ProviderType),
                                                        ConfigurationType = configuration.Value
                                                    })
                                                .ToDictionary(x => x.Metadata, x => x.ConfigurationType);

                       var configurationParser = context.Resolve<ICheckProviderConfigurationParser>();
                       return new CheckProviderConfigurationFactory(configurationTypes, configurationParser);
                   })
                   .As<ICheckProviderConfigurationFactory>()
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterSystemExtractors(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<CheckOutputDataSerializer>()
                   .As<ICheckOutputDataSerializer>()
                   .SingleInstance();

            builder.RegisterType<TaxResidenceOutputDataExtractor>()
                   .As<ICheckOutputDataExtractor>()
                   .WithMetadata<CheckProviderMetadata>(m =>
                   {
                       m.For(p => p.ProviderType, CheckProviderType.System);
                       m.For(p => p.CheckType, CheckType.TaxResidence);
                   });

            builder.RegisterType<IpAddressOutputDataExtractor>()
                   .As<ICheckOutputDataExtractor>()
                   .WithMetadata<CheckProviderMetadata>(m =>
                   {
                       m.For(p => p.ProviderType, CheckProviderType.System);
                       m.For(p => p.CheckType, CheckType.IpMatch);
                   });

            return builder;
        }

        public static ContainerBuilder RegisterSystemCheckProvider(this ContainerBuilder builder, Predicate<IComponentContext> useStub)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Register(c =>
            {
                var countryDetailsProvider = c.Resolve<ICountryDetailsProvider>();
                var matchStrategies = new Dictionary<IpAddressMatchingType, IIpMatchStrategy>
                {
                    { IpAddressMatchingType.ByCountry, new IpMatchByCountryStrategy() },
                    { IpAddressMatchingType.ByState, new IpMatchByStateStrategy() },
                    { IpAddressMatchingType.ByRegion, new IpMatchByRegionStrategy(countryDetailsProvider) }
                };
                return new IpMatchStrategyFactory(matchStrategies);
            });

            builder.RegisterCheckProvider(CheckProviderType.System, options =>
            {
                options.AddFactory<IpAddressCheckProviderFactory>(CheckType.IpMatch, new IpAddressCheckProcessingResultFactory());
                options.AddFactory<SurveyAnswersCheckProviderFactory>(CheckType.SurveyAnswers);
                options.AddFactory<DuplicateScreeningCheckProviderFactory>(CheckType.NameAndDoBDuplication);
                options.AddFactory<IdNumberDuplicationCheckProviderFactory>(CheckType.IdDocNumberDuplication);
                options.AddFactory<TaxResidenceCheckProviderFactory>(CheckType.TaxResidence);
            }, useStub);

            return builder;
        }

        public static ContainerBuilder RegisterSandboxDecorators(this ContainerBuilder builder)
        {
            builder.RegisterDecorator<IDuplicateSearchService>((context, _, inner) =>
            {
                var hostSettingsProvider = context.Resolve<IHostSettingsProvider>();
                var optionsProvider = context.Resolve<IOptionsProvider>();

                var environment = hostSettingsProvider.GetSetting(HostSettingsKey.Environment);
                var isProd = environment.Equals("Production", StringComparison.InvariantCultureIgnoreCase);

                return isProd ? inner : new DuplicateSearchServiceDecorator(inner, optionsProvider);
            });

            return builder;
        }
    }
}
