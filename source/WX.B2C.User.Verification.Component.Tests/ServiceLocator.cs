using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using MbDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Internal.Client;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Helpers;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Mountebank;
using WX.B2C.User.Verification.Component.Tests.Providers;
using WX.KeyVault;

namespace WX.B2C.User.Verification.Component.Tests
{
    internal static class ServiceLocator
    {
        private static IContainer _container;
        private static IConfiguration _configuration;

        static ServiceLocator()
        {
            var containerBuilder = new ContainerBuilder();
            InitConfiguration();
            RegisterTestKeyVault(containerBuilder);
            RegisterBase(containerBuilder);
            RegisterProviders(containerBuilder);
            RegisterFactories(containerBuilder);
            RegisterOptions(containerBuilder);
            RegisterMountebank(containerBuilder);
            _container = containerBuilder.Build();
        }

        public static T Get<T>() => _container.Resolve<T>();

        private static void InitConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                             .SetBasePath(Global.RootPath)
                             .AddJsonFile(Global.SettingsFileName)
                             .Build();
        }

        private static void RegisterBase(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(_ => _configuration).As<IConfiguration>().SingleInstance();
            containerBuilder.RegisterType<StepVariantComparer>().SingleInstance();

            containerBuilder.Register(_ => new DbFixture(_configuration[Global.ConnectionString])).SingleInstance();
            containerBuilder.RegisterType<SurveyFixture>().SingleInstance();
            containerBuilder.RegisterType<TurnoverFixture>().SingleInstance();
            containerBuilder.RegisterType<UserRiskLevelFixture>().SingleInstance();
            containerBuilder.RegisterType<TaskFixture>().SingleInstance();

            containerBuilder.RegisterType<EventsFixture>().SingleInstance();
            containerBuilder.RegisterType<VerificationDetailsFixture>().SingleInstance();
            containerBuilder.RegisterType<ProfileFixture>().SingleInstance();
            containerBuilder.RegisterType<ApplicationFixture>().SingleInstance();
            containerBuilder.RegisterType<CollectionStepsFixture>().SingleInstance();
            containerBuilder.RegisterType<ReminderFixture>().SingleInstance();

            containerBuilder.RegisterType<FileFixture>().SingleInstance();
            containerBuilder.RegisterType<ExternalProfileFixture>().SingleInstance();
            containerBuilder.RegisterType<DocumentsFixture>().SingleInstance();
            containerBuilder.RegisterType<DocumentStepFixture>().SingleInstance();
            containerBuilder.RegisterType<VerificationDetailsStepFixture>().SingleInstance();

            containerBuilder.RegisterType<SurveyStepFixture>().SingleInstance();
            containerBuilder.RegisterType<ExternalProfileFixture>().SingleInstance();
            containerBuilder.RegisterType<ChecksFixture>().SingleInstance();
            containerBuilder.RegisterType<TriggerFixture>().SingleInstance();
        }

        private static void RegisterProviders(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<DocumentProvider>().SingleInstance();
            containerBuilder.RegisterType<HardCodedTriggerProvider>()
                            .As<ITriggerProvider>()
                            .SingleInstance();

            containerBuilder.RegisterType<CheckProvider>()
                            .As<ICheckProvider>()
                            .SingleInstance();

            containerBuilder.RegisterType<CheckDataProvider>()
                            .As<ICheckDataProvider>()
                            .SingleInstance();
        }

        private static void RegisterFactories(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AdministratorFactory>().SingleInstance();
            containerBuilder.Register(_ => new AdminApiClientFactory(_configuration[Global.AdminApiEndpoint])).SingleInstance();
            containerBuilder.Register(_ => new PublicApiClientFactory(_configuration[Global.PublicApiEndpoint])).SingleInstance();
            containerBuilder.Register(_ => new UserVerificationClientSettings { BaseUri = new Uri(_configuration[Global.InternalApiEndpoint]) }).SingleInstance();
            containerBuilder.RegisterType<UserVerificationApiClientFactory>().As<IUserVerificationApiClientFactory>().SingleInstance();
            containerBuilder.Register(_ => new WebhookApiClientFactory(_configuration[Global.WebhookApiEndpoint])).SingleInstance();
        }

        private static void RegisterMountebank(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<MountebankClient>().SingleInstance();
            containerBuilder.Register(context =>
                new OnfidoImposter(context.Resolve<MountebankClient>(),
                                   _configuration[Global.LocalOnfidoApiUrl],
                                   _configuration[Global.OnfidoApiUrl],
                                   _configuration[Global.WebhookApiEndpoint] + "/onfido/events")
            ).As<IOnfidoImposter>().SingleInstance();

            containerBuilder.Register(context =>
                new PassfortImposter(context.Resolve<MountebankClient>(),
                                            _configuration[Global.LocalPassFortApiUrl],
                                            _configuration[Global.PassFortApiUrl],
                                            _configuration[Global.WebhookApiEndpoint] + "/passfort/events")
            ).As<IPassfortImposter>().SingleInstance();

            containerBuilder.Register(context =>
                new SurveyImposter(context.Resolve<MountebankClient>(), _configuration[Global.B2cSurveyApiEndpoint])
            ).As<ISurveyImposter>().SingleInstance();
        }

        private static void RegisterOptions(ContainerBuilder containerBuilder)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new PolymorphicDeserializeJsonConverter<CollectionStepVariantDto>("type"));

            var json = File.ReadAllText(Sources.PathToCheckInfos);
            var checkInfos = JsonConvert.DeserializeObject<IList<CheckInfo>>(json, settings);
            var checkInfosOptions = Options.Create(checkInfos);

            containerBuilder.RegisterInstance(checkInfosOptions).SingleInstance();
        }

        private static void RegisterTestKeyVault(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var keyVaultConfiguration = new KeyVaultConfiguration(
                    configuration["KeyVault:KeyVaultUrl"],
                    configuration["KeyVault:KeyVaultClientId"],
                    configuration["KeyVault:KeyVaultSecret"]);
                return KeyVaultProxy<ITestKeyVault>.Create(keyVaultConfiguration);
            }).As<ITestKeyVault>().SingleInstance();
        }
    }
}
