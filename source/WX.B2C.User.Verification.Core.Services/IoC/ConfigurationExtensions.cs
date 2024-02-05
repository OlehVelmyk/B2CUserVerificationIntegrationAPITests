using System;
using Autofac;
using Serilog.Events;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Core.Services.Configuration;
using WX.B2C.User.Verification.Core.Services.Mappers;
using WX.B2C.User.Verification.Core.Services.Providers;
using WX.B2C.User.Verification.Core.Services.RequiredData;
using WX.B2C.User.Verification.Core.Services.Tickets;
using WX.B2C.User.Verification.Core.Services.UserEmails;
using WX.B2C.User.Verification.Domain.Shared;
using WX.Logging.Autofac;

namespace WX.B2C.User.Verification.Core.Services.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterCoreServices(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterInternalMappers();

            builder.RegisterType<CheckService>()
                   .As<ICheckService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<CollectionStepService>()
                   .As<ICollectionStepService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<TaskService>()
                   .As<ITaskService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<ProfilePatcher>()
                   .As<IProfilePatcher>()
                   .SingleInstance();

            builder.RegisterType<ProfileService>()
                   .As<IProfileService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<DocumentService>()
                   .As<IDocumentService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<FileService>()
                   .As<IFileService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<ApplicationService>()
                   .As<IApplicationService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<ExternalProfileProvider>()
                   .As<IExternalProfileProvider>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<TriggerService>()
                   .As<ITriggerService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<ProfileProviderFactory>()
                   .As<IProfileProviderFactory>()
                   .SingleInstance();

            builder.RegisterConfigurations()
                   .RegisterCommonServices();

            return builder;
        }

        public static ContainerBuilder RegisterTicketSender(this ContainerBuilder builder)
        {
            builder.RegisterType<HardCodedTicketInfoProvider>()
                   .As<ITicketInfoProvider>()
                   .SingleInstance();

            builder.RegisterType<TicketService>()
                   .As<ITicketService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<TicketFactory>()
                   .As<ITicketFactory>()
                   .SingleInstance();

            builder.RegisterType<TicketParametersReader>()
                   .As<ITicketParametersReader>()
                   .SingleInstance();

            builder.RegisterExternalLinkProvider();
            builder.RegisterCommonServices();

            return builder;
        }

        /// <summary>
        /// TODO put here common classes which used everywhere, like system clock, etc0
        /// </summary>
        public static ContainerBuilder RegisterCommonServices(this ContainerBuilder builder)
        {
            builder.RegisterType<XPathParser>()
                   .As<IXPathParser>()
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterSystemClock(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<DefaultSystemClock>()
                   .As<ISystemClock>()
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterExternalLinkProvider(this ContainerBuilder builder)
        {
            builder.RegisterType<ExternalLinkProvider>()
                   .As<IExternalLinkProvider>()
                   .SingleInstance();

            return builder;
        }
        
        /// <summary>
        /// Note that <see cref="IFileProvider"/> uses Onfido for downloading like fallback strategy.
        /// Therefore registering Onfido gateway is required
        /// </summary>
        public static ContainerBuilder RegisterFileProvider(this ContainerBuilder builder)
        { 
            builder.RegisterInternalMappers();   

            builder.RegisterType<FileProvider>()
                   .As<IFileProvider>()
                   .SingleInstance();

               return builder;
        }

        public static ContainerBuilder RegisterConfigurations(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));


            builder.RegisterType<FeatureToggleService>()
                   .As<IFeatureToggleService>()
                   .SingleInstance();

            builder.RegisterType<CountryDetailsProvider>()
                   .As<ICountryDetailsProvider>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<StatesDetailsProvider>()
                   .As<IStatesDetailsProvider>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<OptionsProvider>()
                   .As<IOptionsProvider>()
                   .SingleInstance();

            builder.RegisterType<HardcodedSpecificOptionsProvider>()
                   .As<IOptionProvider<SpecificCountriesOption>>()
                   .As<IOptionProvider>()
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterUserEmailService(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<UserEmailService>()
                   .As<IUserEmailService>()
                   .SingleInstance();
            
            builder.RegisterType<ParametersProvider>()
                   .As<IParametersProvider>()
                   .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterInternalMappers(this ContainerBuilder builder)
        {
            builder.RegisterType<InitiationMapper>()
                   .As<IInitiationMapper>()
                   .SingleInstance();
            
            builder.RegisterType<BlobFileMapper>()
                   .As<IBlobFileMapper>()
                   .SingleInstance();

            return builder;
        }
    }
}