using System;
using System.Diagnostics;
using Autofac;
using Autofac.Features.Indexed;
using Quartz;
using Quartz.Spi;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.B2C.User.Verification.Worker.Jobs.Clients;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using WX.B2C.User.Verification.Worker.Jobs.Jobs;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Providers.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks;
using WX.B2C.User.Verification.Worker.Jobs.Quartz;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using WX.Core.TypeExtensions;
using WX.Logging.Autofac;
using ICollectionStepRepository = WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories.ICollectionStepRepository;

namespace WX.B2C.User.Verification.Worker.Jobs.IoC
{
    internal static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterQuartzScheduler(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<JobFactory>()
                   .As<IJobFactory>()
                   .SingleInstance();

            builder.RegisterType<QuartzPropertiesProvider>()
                   .As<IQuartzPropertiesProvider>()
                   .SingleInstance();

            builder.RegisterType<JobSchedulerInitializer>()
                   .As<IJobSchedulerInitializer>()
                   .SingleInstance();

            builder.RegisterType<JobSchedulerFactory>()
                   .AsSelf()
                   .SingleInstance();

            builder.Register(provider =>
                   {
                       //TODO improve retry mechanism
                       //var retrySettings = new JobRetrySettings(3, TimeSpan.FromMinutes(5));
                       //var retryStrategy = new ExponentialBackoffRetryStrategy(retrySettings);
                       var schedulerFactory = provider.Resolve<JobSchedulerFactory>();
                       return schedulerFactory.Create().GetAwaiter().GetResult();
                   })
                   .As<IScheduler>()
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterJobServices(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<JobBuilderProvider>()
                   .As<IJobBuilderProvider>()
                   .SingleInstance();

            builder.RegisterType<TriggerKeyProvider>()
                   .As<ITriggerKeyProvider>()
                   .SingleInstance();

            builder.RegisterType<JobKeyProvider>()
                   .As<IJobKeyProvider>()
                   .SingleInstance();

            builder.RegisterType<JobService>()
                   .As<IJobService>()
                   .SingleInstance()
                   .UseCallLogger();

            builder.RegisterInstance(new BridgerPasswordOptions());
            builder.RegisterType<BridgerPasswordGenerator>()
                   .As<IBridgerPasswordGenerator>()
                   .SingleInstance();

            builder.RegisterType<CollectionStepCreatorFactory>()
                   .As<ICollectionStepCreatorFactory>()
                   .SingleInstance();

            builder.RegisterType<TaskVariantMediator>()
                   .As<ITaskVariantProvider>()
                   .SingleInstance();

            builder.RegisterType<StaticTasksProvider>()
                   .As<IStaticTasksProvider>()
                   .SingleInstance();

            builder.RegisterType<HardcodedDynamicTasksProvider>()
                   .As<IDynamicTasksProvider>()
                   .SingleInstance();

            //Has state. Must be not single instance
            builder.RegisterType<ReportBuilder>()
                   .As<IReportBuilder>();

            return builder;
        }

        public static ContainerBuilder RegisterJobValidators(this ContainerBuilder builder)
        {
            builder.RegisterType<UserConsistencyValidator>().AsSelf().SingleInstance();
            return builder;
        }

        public static ContainerBuilder RegisterDbFactories(this ContainerBuilder builder)
        {
            builder.RegisterType<B2CUserVerificationQueryFactory>().As<IQueryFactory>().SingleInstance();
            builder.RegisterType<UserVerificationQueryFactory>().As<IUserVerificationQueryFactory>().SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterJobRepositories(this ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationRepository>().As<IApplicationRepository>().SingleInstance();
            builder.RegisterType<TaskCollectionStepRepository>().As<ITaskCollectionStepRepository>().SingleInstance();
            builder.RegisterType<TaskRepository>().As<ITaskRepository>().SingleInstance();
            builder.RegisterType<CollectionStepRepository>().As<ICollectionStepRepository>().SingleInstance();
            builder.RegisterType<CheckRepository>().As<ICheckRepository>().SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterJobDataAggregationServices(this ContainerBuilder builder)
        {
            builder.RegisterType<TaskCreatingDataAggregationService>().As<ITaskCreatingDataAggregationService>().SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterJobProviders(this ContainerBuilder builder)
        {
            builder.RegisterType<CsvEntityProvider>()
                   .As<ICsvEntityProvider>()
                   .SingleInstance();
            
            builder.RegisterGeneric(typeof(CsvBlobDataProvider<>))
                   .As(typeof(ICsvBlobDataProvider<>))
                   .SingleInstance();
            
            builder.RegisterType<FileDataProvider>()
                   .As<IFileDataProvider>()
                   .SingleInstance();

            builder.RegisterType<ApplicantDataProvider>()
                   .As<IApplicantDataProvider>()
                   .SingleInstance();

            builder.RegisterType<OnfidoFileDataProvider>()
                   .As<IOnfidoFileDataProvider>()
                   .SingleInstance();

            builder.RegisterType<UsaApplicationDataProvider>()
                   .As<IUsaApplicationDataProvider>()
                   .SingleInstance();

            builder.RegisterType<ApplicationDataProvider>()
                   .As<IApplicationDataProvider>()
                   .SingleInstance();

            builder.RegisterType<ProofOfFundsCheckDataProvider>()
                   .As<IProofOfFundsCheckDataProvider>()
                   .SingleInstance();

            builder.RegisterType<TaskDataProvider>()
                   .As<ITaskDataProvider>()
                   .SingleInstance();

            builder.RegisterType<FinancialConditionProvider>()
                   .As<IFinancialConditionProvider>()
                   .SingleInstance();

            builder.RegisterType<ProofOfAddressProvider>()
                   .As<IProofOfAddressProvider>()
                   .SingleInstance();

            builder.RegisterType<TaxResidenceProvider>()
                   .As<ITaxResidenceProvider>()
                   .SingleInstance();

            builder.RegisterType<TaxResidenceAddressProvider>()
                   .As<ITaxResidenceAddressProvider>()
                   .SingleInstance();

            builder.RegisterType<DocumentsChecksProvider>()
                   .As<IDocumentsChecksProvider>()
                   .SingleInstance();

            builder.RegisterType<ProfileDataExistenceProvider>()
                   .As<IProfileDataExistenceProvider>()
                   .SingleInstance();

            builder.RegisterType<UserSurveyChecksProvider>()
                   .As<IUserSurveyChecksProvider>()
                   .SingleInstance();

            builder.RegisterType<AccountAlertInfoProvider>()
                   .As<IAccountAlertInfoProvider>();

            builder.RegisterType<InstructChecksDataProvider>()
                   .As<IInstructChecksDataProvider>()
                   .SingleInstance();

            builder.RegisterType<TaskCreatingDataProvider>()
                   .As<ITaskCreatingDataProvider>()
                   .SingleInstance();
            
            builder.RegisterType<UserDefectsModelProvider>()
                   .As<IUserDefectsModelProvider>()
                   .SingleInstance();  
            
            builder.RegisterType<CreateStepStateJobProvider>()
                   .As<ICreateStepStateJobProvider>()
                   .SingleInstance();      
            
            builder.RegisterType<ChecksDataProvider>()
                   .As<IChecksDataProvider>()
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterJobs(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterJob<RefreshBridgerPasswordJob>();
            builder.RegisterJob<ScheduledTriggerJob>();
            builder.RegisterJob<FileValidationJob>();
            builder.RegisterJob<OnfidoDocumentOcrJob>();
            builder.RegisterJob<SelfieJob>();
            builder.RegisterJob<FraudScreeningTaskJob>();
            builder.RegisterJob<RiskListsScreeningTaskJob>();
            builder.RegisterJob<FinancialConditionJob>();
            builder.RegisterJob<ProofOfAddressJob>();
            builder.RegisterJob<ProofOfFundsTaskJob>();
            builder.RegisterJob<TaxResidenceJob>();
            builder.RegisterJob<TaskStateJob>();
            builder.RegisterJob<ProfileCollectionStepsJob>();
            builder.RegisterJob<SurveyCollectionStepsJob>();
            builder.RegisterJob<DocumentsCollectionStepsJob>();
            builder.RegisterJob<AccountAlertJob>();
            builder.RegisterJob<TriggerActionsRequestingJob>();
            builder.RegisterJob<InstructChecksJob>();
            builder.RegisterJob<TaskCreatingJob>();
            builder.RegisterJob<AutomateApplicationJob>();
            builder.RegisterJob<DefectDetectingJob>();
            builder.RegisterJob<UpdateStepStateJob>();
            builder.RegisterJob<CreateStepStateJob>();
            builder.RegisterJob<DeleteStepStateJob>();
            builder.RegisterJob<RerunCheckJob>();
            builder.RegisterJob<CancelCheckJob>();
            builder.RegisterJob<UserReminderJob>();

            builder.RegisterDecorator<JobExecutionLoggingDecorator, IJob>();
            builder.RegisterDecorator<OperationContextJobExecutionDecorator, IJob>();
            builder.RegisterDecorator<EnsureJobExecutionExceptionDecorator, IJob>();
            return builder;
        }

        public static ContainerBuilder RegisterJobBuilders(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterJobBuilder(RefreshBridgerPasswordJob.Name, RefreshBridgerPasswordJob.Builder);
            builder.RegisterJobBuilder(ScheduledTriggerJob.Name, ScheduledTriggerJob.Builder);
            builder.RegisterJobBuilder(FileValidationJob.Name, FileValidationJob.Builder);
            builder.RegisterJobBuilder(OnfidoDocumentOcrJob.Name, OnfidoDocumentOcrJob.Builder);
            builder.RegisterJobBuilder(SelfieJob.Name, SelfieJob.Builder);
            builder.RegisterJobBuilder(FraudScreeningTaskJob.Name, FraudScreeningTaskJob.Builder);
            builder.RegisterJobBuilder(RiskListsScreeningTaskJob.Name, RiskListsScreeningTaskJob.Builder);
            builder.RegisterJobBuilder(TaskStateJob.Name, TaskStateJob.Builder);
            builder.RegisterJobBuilder(FinancialConditionJob.Name, FinancialConditionJob.Builder);
            builder.RegisterJobBuilder(ProofOfAddressJob.Name, ProofOfAddressJob.Builder);
            builder.RegisterJobBuilder(ProofOfFundsTaskJob.Name, ProofOfFundsTaskJob.Builder);
            builder.RegisterJobBuilder(TaxResidenceJob.Name, TaxResidenceJob.Builder);
            builder.RegisterJobBuilder(ProfileCollectionStepsJob.Name, ProfileCollectionStepsJob.Builder);
            builder.RegisterJobBuilder(SurveyCollectionStepsJob.Name, SurveyCollectionStepsJob.Builder);
            builder.RegisterJobBuilder(DocumentsCollectionStepsJob.Name, DocumentsCollectionStepsJob.Builder);
            builder.RegisterJobBuilder(AccountAlertJob.Name, AccountAlertJob.Builder);
            builder.RegisterJobBuilder(TriggerActionsRequestingJob.Name, TriggerActionsRequestingJob.Builder);
            builder.RegisterJobBuilder(InstructChecksJob.Name, InstructChecksJob.Builder);
            builder.RegisterJobBuilder(TaskCreatingJob.Name, TaskCreatingJob.Builder);
            builder.RegisterJobBuilder(AutomateApplicationJob.Name, AutomateApplicationJob.Builder);
            builder.RegisterJobBuilder(DefectDetectingJob.Name, DefectDetectingJob.Builder);
            builder.RegisterJobBuilder(UpdateStepStateJob.Name, UpdateStepStateJob.Builder);
            builder.RegisterJobBuilder(CreateStepStateJob.Name, CreateStepStateJob.Builder);
            builder.RegisterJobBuilder(DeleteStepStateJob.Name, DeleteStepStateJob.Builder);
            builder.RegisterJobBuilder(RerunCheckJob.Name, RerunCheckJob.Builder);
            builder.RegisterJobBuilder(CancelCheckJob.Name, CancelCheckJob.Builder);
            builder.RegisterJobBuilder(UserReminderJob.Name, UserReminderJob.Builder);

            return builder;
        }

        public static ContainerBuilder RegisterTriggerKeyFactories(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterTriggerKeyFactory(ScheduledTriggerJob.Name, ScheduledTriggerJob.TriggerKeyFactory);
            builder.RegisterTriggerKeyFactory(UserReminderJob.Name, UserReminderJob.TriggerKeyFactory);

            return builder;
        }
        
        public static ContainerBuilder RegisterJobKeyFactories(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterJobKeyFactory(UserReminderJob.Name, UserReminderJob.JobKeyFactory);

            return builder;
        }

        public static ContainerBuilder RegisterOnfidoThrottler(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<ThrottledOnfidoApiClientFactory>().As<IThrottledOnfidoApiClientFactory>().SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterJobConfigs(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterJobConfig(c =>
            {
                var hostSettingsProvider = c.Resolve<IHostSettingsProvider>();
                var keyVault = c.Resolve<ILexisNexisBridgerKeyVault>();

                return new BridgerPasswordRefreshSetting
                {
                    DaysBeforeChangePassword = int.Parse(hostSettingsProvider.GetSetting("DaysBeforeChangeBridgerPassword")),
                    UserId = keyVault.UserId.UnSecure()
                };
            });

            builder.RegisterJobConfig(c =>
            {
                var hostSettingsProvider = c.Resolve<IHostSettingsProvider>();
                return new AccountAlertJobConfig
                {
                    ApplicationState = ApplicationState.Approved,
                    ExcludedCountries = hostSettingsProvider.GetSetting("ExcludedCountries").Split(","),
                    Periods = new[]
                    {
                        new AlertPeriod 
                        { 
                            RiskLevel = RiskLevel.ExtraHigh,
                            AccountAge = int.Parse(hostSettingsProvider.GetSetting("ExtraHighAccountAge")),
                            OverallTurnover = decimal.Parse(hostSettingsProvider.GetSetting("ExtraHighTurnover"))
                        },
                        new AlertPeriod 
                        { 
                            RiskLevel = RiskLevel.High, 
                            AccountAge = int.Parse(hostSettingsProvider.GetSetting("HighAccountAge")), 
                            OverallTurnover = decimal.Parse(hostSettingsProvider.GetSetting("HighTurnover")) 
                        },
                        new AlertPeriod 
                        { 
                            RiskLevel = RiskLevel.Medium, 
                            AccountAge = int.Parse(hostSettingsProvider.GetSetting("MediumAccountAge")), 
                            OverallTurnover = decimal.Parse(hostSettingsProvider.GetSetting("MediumTurnover")) 
                        },
                        new AlertPeriod 
                        { 
                            RiskLevel = RiskLevel.Low, 
                            AccountAge = int.Parse(hostSettingsProvider.GetSetting("LowAccountAge")), 
                            OverallTurnover = decimal.Parse(hostSettingsProvider.GetSetting("LowTurnover")) 
                        }
                    }
                };
            });

            return builder;
        }

        private static void RegisterJob<TJob>(this ContainerBuilder builder) where TJob : IJob =>
            builder.RegisterType<TJob>()
                   .AsSelf()
                   .Keyed<IJob>(typeof(TJob));

        private static void RegisterJobBuilder(this ContainerBuilder builder, string jobName, JobBuilderFactory jobBuilderFactory) =>
            builder.RegisterInstance(jobBuilderFactory)
                   .AsImplementedInterfaces()
                   .SingleInstance()
                   .Keyed<JobBuilderFactory>(jobName);

        private static void RegisterTriggerKeyFactory(this ContainerBuilder builder, string jobName, TriggerKeyFactory factory) =>
            builder.RegisterInstance(factory)
                   .AsImplementedInterfaces()
                   .SingleInstance()
                   .Keyed<TriggerKeyFactory>(jobName);
        
        private static void RegisterJobKeyFactory(this ContainerBuilder builder, string jobName, JobKeyFactory factory) =>
            builder.RegisterInstance(factory)
                   .AsImplementedInterfaces()
                   .SingleInstance()
                   .Keyed<JobKeyFactory>(jobName);

        private static void RegisterJobConfig<T>(this ContainerBuilder builder, Func<IComponentContext, T> func) =>
            builder.Register(func).AsSelf();
    }
}