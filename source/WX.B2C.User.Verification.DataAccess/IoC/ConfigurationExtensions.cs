using System;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.DataAccess.Repositories;
using WX.B2C.User.Verification.DataAccess.Storages;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.Core.TypeExtensions;
using WX.Logging.Autofac;

namespace WX.B2C.User.Verification.DataAccess.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterDataAccess(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.RegisterDbContext()
                          .RegisterMappers()
                          .RegisterStorages()
                          .RegisterRepositories()
                          .RegisterDeserializer();
        }

        private static ContainerBuilder RegisterDeserializer(this ContainerBuilder builder)
        {
            builder.RegisterType<PolicyObjectsDeserializer>()
                   .As<IPolicyObjectsDeserializer>();

            return builder;
        }

        private static ContainerBuilder RegisterDbContext(this ContainerBuilder builder)
        {
            builder.Register(context =>
                   {
                       var appConfig = context.Resolve<IAppConfig>();
                       var logger = context.Resolve<ILogger>();

                       var optionsBuilder = new DbContextOptionsBuilder<VerificationDbContext>();
                       optionsBuilder.UseSqlServer(appConfig.DbConnectionString.UnSecure(), options => {
                           options.ExecutionStrategy(dependencies =>
                               new SqlServerRetryingExecutionStrategy(logger, dependencies));
                       });

                       return optionsBuilder.Options;
                   })
                   .As<DbContextOptions>()
                   .SingleInstance();

            builder.RegisterType<DbContextFactory>()
                   .As<IDbContextFactory>()
                   .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterMappers(this ContainerBuilder builder)
        {
            builder.RegisterType<NoteMapper>()
                   .As<INoteMapper>()
                   .SingleInstance();

            builder.RegisterType<PolicyMapper>()
                   .As<IPolicyMapper>()
                   .SingleInstance();

            builder.RegisterType<PersonalDetailsMapper>()
                   .As<IPersonalDetailsMapper>()
                   .SingleInstance();

            builder.RegisterType<VerificationDetailsMapper>()
                   .As<IVerificationDetailsMapper>()
                   .SingleInstance();

            builder.RegisterType<DocumentMapper>()
                   .As<IDocumentMapper>()
                   .SingleInstance();

            builder.RegisterType<FileMapper>()
                   .As<IFileMapper>()
                   .SingleInstance();

            builder.RegisterType<AuditEntityMapper>()
                   .As<IAuditEntityMapper>()
                   .SingleInstance();

            builder.RegisterType<CollectionStepMapper>()
                   .As<ICollectionStepMapper>()
                   .SingleInstance();

            builder.RegisterType<TaskMapper>()
                   .As<ITaskMapper>()
                   .SingleInstance();

            builder.RegisterType<CheckMapper>()
                   .As<ICheckMapper>()
                   .SingleInstance();

            builder.RegisterType<ApplicationMapper>()
                   .As<IApplicationMapper>()
                   .SingleInstance();

            builder.RegisterType<ApplicationStateChangelogMapper>()
                   .As<IApplicationStateChangelogMapper>()
                   .SingleInstance();

            builder.RegisterType<ExternalProfileMapper>()
                   .As<IExternalProfileMapper>()
                   .SingleInstance();

            builder.RegisterType<BridgerCredentialsMapper>()
                   .As<IBridgerCredentialsMapper>()
                   .SingleInstance();

            builder.RegisterType<ValidationRuleMapper>()
                   .As<IValidationRuleMapper>()
                   .SingleInstance();            
            
            builder.RegisterType<TriggerMapper>()
                   .As<ITriggerMapper>()
                   .SingleInstance();          
            
            builder.RegisterType<TriggerVariantMapper>()
                   .As<ITriggerVariantMapper>()
                   .SingleInstance();

            builder.RegisterType<MonitoringPolicyMapper>()
                   .As<IMonitoringPolicyMapper>()
                   .SingleInstance();

            builder.RegisterType<ReminderMapper>()
                   .As<IReminderMapper>()
                   .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterStorages(this ContainerBuilder builder)
        {
            builder.RegisterType<NoteStorage>()
                   .As<INoteStorage>()
                   .SingleInstance();

            builder.RegisterType<VerificationPolicyStorage>()
                   .As<IVerificationPolicyStorage>()
                   .SingleInstance();

            builder.RegisterType<DuplicateSearchService>()
                   .As<IDuplicateSearchService>()
                   .SingleInstance();

            builder.RegisterType<CollectionStepStorage>()
                   .As<ICollectionStepStorage>()
                   .SingleInstance();

            builder.RegisterType<TaskStorage>()
                   .As<ITaskStorage>()
                   .SingleInstance();

            builder.RegisterType<ApplicationStorage>()
                   .As<IApplicationStorage>()
                   .SingleInstance();

            builder.RegisterType<ApplicationStateChangelogStorage>()
                   .As<IApplicationStateChangelogStorage>()
                   .SingleInstance();

            builder.RegisterType<ProfileStorage>()
                   .As<IProfileStorage>()
                   .SingleInstance();

            builder.RegisterType<CheckStorage>()
                   .As<ICheckStorage>()
                   .SingleInstance();

            builder.RegisterType<DocumentStorage>()
                   .As<IDocumentStorage>()
                   .SingleInstance();

            builder.RegisterType<FileStorage>()
                   .As<IFileStorage>()
                   .SingleInstance();

            builder.RegisterType<CheckProviderConfigurationStorage>()
                   .As<ICheckProviderConfigurationStorage>()
                   .SingleInstance();

            builder.RegisterType<BridgerCredentialsStorage>()
                   .As<IBridgerCredentialsStorage>()
                   .SingleInstance();

            builder.RegisterType<ValidationPolicyStorage>()
                   .As<IValidationPolicyStorage>()
                   .SingleInstance();

            builder.RegisterType<TriggerVariantStorage>()
                   .As<ITriggerVariantStorage>()
                   .SingleInstance();
                   
            builder.RegisterType<MonitoringPolicyStorage>()
                   .As<IMonitoringPolicyStorage>()
                   .SingleInstance();

            builder.RegisterType<AuditEntryStorage>()
                   .As<IAuditEntryStorage>()
                   .SingleInstance();

            builder.RegisterType<TriggerStorage>()
                   .As<ITriggerStorage>()
                   .SingleInstance();

            builder.RegisterType<HardcodedDocumentTypeProvider>()
                   .As<IDocumentTypeProvider>()
                   .SingleInstance();

            builder.RegisterType<ExternalProfileStorage>()
                   .As<IExternalProfileStorage>()
                   .SingleInstance();

            builder.RegisterType<ReminderStorage>()
                   .As<IReminderStorage>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            return builder;
        }

        private static ContainerBuilder RegisterRepositories(this ContainerBuilder builder)
        {
            builder.RegisterType<NoteRepository>()
                   .As<INoteRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<PersonalDetailsRepository>()
                   .As<IPersonalDetailsRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<VerificationDetailsRepository>()
                   .As<IVerificationDetailsRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<DocumentRepository>()
                   .As<IDocumentRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<FileRepository>()
                   .As<IFileRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<AuditEntryRepository>()
                   .As<IAuditEntryRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<CollectionStepRepository>()
                   .As<ICollectionStepRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<TaskRepository>()
                   .As<ITaskRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<CheckRepository>()
                   .As<ICheckRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<ApplicationRepository>()
                   .As<IApplicationRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<ApplicationStateChangelogRepository>()
                   .As<IApplicationStateChangelogRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<ExternalProfileRepository>()
                   .As<IExternalProfileRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<BridgerCredentialsRepository>()
                   .As<IBridgerCredentialsRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<TriggerRepository>()
                   .As<ITriggerRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<ReminderRepository>()
                   .As<IReminderRepository>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            return builder;
        }
    }
}
