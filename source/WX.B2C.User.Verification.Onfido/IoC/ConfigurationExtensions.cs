using System;
using Autofac;
using FluentValidation;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Module;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Extractors;
using WX.B2C.User.Verification.Onfido.Factories;
using WX.B2C.User.Verification.Onfido.Mappers;
using WX.B2C.User.Verification.Onfido.Processors;
using WX.B2C.User.Verification.Onfido.Processors.Validators;
using WX.B2C.User.Verification.Onfido.Sandbox;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.IoC;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Onfido.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterOnfidoGateway(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterClientFactories()
                   .RegisterMappers();

            builder.RegisterType<OnfidoTokenProvider>()
                   .As<IOnfidoTokenProvider>()
                   .SingleInstance();

            builder.RegisterType<FileProvider>()
                   .Keyed<IExternalFileProvider>(Core.Contracts.Enum.ExternalFileProviderType.Onfido)
                   .SingleInstance();

            builder.RegisterType<OnfidoApplicantFactory>()
                   .Keyed<IExternalProfileFactory>(ExternalProviderType.Onfido)
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterOnfidoProvider(this ContainerBuilder builder, Predicate<IComponentContext> shouldUseStub)
        {
            builder.RegisterClientFactories()
                   .RegisterMappers();

            builder.RegisterCheckProvider(CheckProviderType.Onfido, options =>
            {
                options.AddFactory<IdentityDocumentsCheckProviderFactory>(CheckType.IdentityDocument);
                options.AddFactory<FacialSimilarityCheckProviderFactory>(CheckType.FacialSimilarity);
                options.AddFactory<FaceDuplicationCheckProviderFactory>(CheckType.FaceDuplication);
                options.AddFactory<IdentityEnhancedCheckProviderFactory>(CheckType.IdentityEnhanced);
                options.AddComplexFactory<ComplexCheckProviderFactory>();
            }, shouldUseStub);

            builder.RegisterType<DocumentReportValidator>()
                   .Keyed<IValidator>(typeof(DocumentReport))
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterType<FacialSimilarityReportValidator>()
                   .Keyed<IValidator>(typeof(FacialSimilarityPhotoReport))
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterType<KnownFacesReportValidator>()
                   .Keyed<IValidator>(typeof(KnownFacesReport))
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterType<IdentityDocumentCheckResultProcessor>()
                   .As<IIdentityDocumentCheckResultProcessor>()
                   .SingleInstance();

            builder.RegisterDecorator<IIdentityDocumentCheckResultProcessor>((context, _, processor) =>
            {
                var hostSettingsProvider = context.Resolve<IHostSettingsProvider>();
                var profileStorage = context.Resolve<IProfileStorage>();

                var environment = hostSettingsProvider.GetSetting(HostSettingsKey.Environment);
                var isProd = environment.Equals("Production", StringComparison.InvariantCultureIgnoreCase);
                var isLocal = environment.Equals("Local", StringComparison.InvariantCultureIgnoreCase);

                if (isProd || isLocal)
                    return processor;

                return new IdentityDocumentProcessorDecorator(profileStorage, processor);
            });

            builder.RegisterType<IdentityEnhancedCheckResultProcessor>()
                   .As<IIdentityEnhancedCheckResultProcessor>()
                   .SingleInstance();

            builder.RegisterType<FaceDuplicationCheckResultProcessor>()
                   .As<IFaceDuplicationCheckResultProcessor>()
                   .SingleInstance();

            builder.RegisterType<FacialSimilarityCheckResultProcessor>()
                   .As<IFacialSimilarityCheckResultProcessor>()
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterOnfidoExtractors(this ContainerBuilder builder)
        {
            builder.RegisterType<IdentityCheckOutputDataExtractor>()
                   .As<ICheckOutputDataExtractor>()
                   .WithMetadata<CheckProviderMetadata>(m =>
                   {
                       m.For(p => p.ProviderType, CheckProviderType.Onfido);
                       m.For(p => p.CheckType, CheckType.IdentityDocument);
                   })
                   .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterMappers(this ContainerBuilder builder)
        {
            builder.RegisterType<OnfidoDocumentMapper>()
                   .As<IOnfidoDocumentMapper>()
                   .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterClientFactories(this ContainerBuilder builder)
        {
            builder.RegisterType<OnfidoPolicyFactory>()
                   .As<IOnfidoPolicyFactory>()
                   .SingleInstance();

            builder.Register(context =>
                   {
                       var appConfig = context.Resolve<IAppConfig>();
                       var onfidoPolicyFactory = context.Resolve<IOnfidoPolicyFactory>();
                       var onfidoClientSettings = new OnfidoClientSettings(appConfig.OnfidoApiUrl, appConfig.OnfidoApiToken.UnSecure());

                       return new OnfidoApiClientFactory(onfidoPolicyFactory, onfidoClientSettings);
                   })
                   .As<IOnfidoApiClientFactory>()
                   .SingleInstance();

            return builder;
        }
    }
}