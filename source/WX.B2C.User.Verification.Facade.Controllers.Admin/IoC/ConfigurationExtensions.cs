using System;
using Autofac;
using Microsoft.AspNetCore.Http;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Services;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterAdminFacade(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder
                   .RegisterMappers()
                   .RegisterServices()
                   .RegisterAsyncValidators();
        }

        private static ContainerBuilder RegisterMappers(this ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationMapper>()
                   .As<IApplicationMapper>()
                   .SingleInstance();

            builder.RegisterType<CheckMapper>()
                   .As<ICheckMapper>()
                   .SingleInstance();

            builder.RegisterType<DocumentMapper>()
                   .As<IDocumentMapper>()
                   .SingleInstance();

            builder.RegisterType<CollectionStepMapper>()
                   .As<ICollectionStepMapper>()
                   .SingleInstance();

            builder.RegisterType<TaskMapper>()
                   .As<ITaskMapper>()
                   .SingleInstance();

            builder.RegisterType<VerificationDetailsMapper>()
                   .As<IVerificationDetailsMapper>()
                   .SingleInstance();

            builder.RegisterType<InitiationMapper>()
                   .As<IInitiationMapper>()
                   .SingleInstance();

            builder.RegisterType<ContentTypeMapper>()
                   .As<IContentTypeMapper>()
                   .SingleInstance();

            builder.RegisterType<AuditMapper>()
                   .As<IAuditMapper>()
                   .SingleInstance();

            builder.RegisterType<JobMapper>()
                   .As<IJobMapper>()
                   .SingleInstance();

            builder.RegisterType<ExternalProfileMapper>()
                   .As<IExternalProfileMapper>()
                   .SingleInstance();

            builder.RegisterType<SurveyMapper>()
                   .As<ISurveyMapper>()
                   .SingleInstance();

            builder.RegisterType<DocumentCategoryMapper>()
                   .As<IDocumentCategoryMapper>()
                   .SingleInstance();

            builder.RegisterType<CollectionStepVariantMapper>()
                   .As<ICollectionStepVariantMapper>()
                   .SingleInstance();

            builder.RegisterType<CollectionStepNameMapper>()
                   .As<ICollectionStepNameMapper>()
                   .SingleInstance();

            builder.RegisterType<CollectionStepDataMapper>()
                   .As<ICollectionStepDataMapper>()
                   .SingleInstance();

            builder.RegisterType<CollectionStepBriefDataMapper>()
                   .As<ICollectionStepBriefDataMapper>()
                   .SingleInstance();

            builder.RegisterType<FileMapper>()
                   .As<IFileMapper>()
                   .SingleInstance();

            builder.RegisterType<NoteMapper>()
                   .As<INoteMapper>()
                   .SingleInstance();

            builder.RegisterType<PersonalDetailsMapper>()
                   .As<IPersonalDetailsMapper>()
                   .SingleInstance();

            builder.RegisterType<ConfigurationMapper>()
                   .As<IConfigurationMapper>()
				   .SingleInstance();
				   
            builder.RegisterType<ReminderMapper>()
                   .As<IReminderMapper>()
                   .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterServices(this ContainerBuilder builder)
        {
            builder.RegisterType<CollectionStepAggregationService>()
                   .As<ICollectionStepAggregationService>()
                   .SingleInstance();

            builder.RegisterType<CheckAggregationService>()
                   .As<ICheckAggregationService>()
                   .SingleInstance();

            builder.RegisterType<TaskAggregationService>()
                   .As<ITaskAggregationService>()
                   .SingleInstance();

            builder.RegisterType<ApplicationAggregationService>()
                   .As<IApplicationAggregationService>()
                   .SingleInstance();

            builder.RegisterType<HttpContextAccessor>()
                   .As<IHttpContextAccessor>()
                   .SingleInstance();

            builder.RegisterType<AdminInitiationProvider>()
                   .As<IInitiationProvider>()
                   .SingleInstance();

            builder.RegisterType<ApplicationActionsManager>()
                   .As<IApplicationActionsManager>()
                   .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterAsyncValidators(this ContainerBuilder builder)
        {
            return builder;
        }
    }
}
