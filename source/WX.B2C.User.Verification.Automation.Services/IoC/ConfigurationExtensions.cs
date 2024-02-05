using System;
using Autofac;
using Serilog.Events;
using WX.B2C.User.Verification.Automation.Services.Approve;
using WX.B2C.User.Verification.Automation.Services.Commands;
using WX.B2C.User.Verification.Automation.Services.Conditions;
using WX.B2C.User.Verification.Automation.Services.Mappers;
using WX.B2C.User.Verification.Automation.Services.Monitoring;
using WX.B2C.User.Verification.Automation.Services.Triggers;
using WX.B2C.User.Verification.Automation.Services.Validators;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Core.Contracts.Commands;
using WX.B2C.User.Verification.Core.Contracts.Conditions;
using WX.B2C.User.Verification.Core.Contracts.Monitoring;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.Logging.Autofac;

namespace WX.B2C.User.Verification.Automation.Services.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterAutomationServices(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterApprovalConditions();
            builder.RegisterConditions();
            builder.RegisterMonitoringServices();
            builder.RegisterApplicationBuilder();

            builder.RegisterType<BatchCommandPublisher>()
                   .As<IBatchCommandPublisher>()
                   .SingleInstance();

            builder.RegisterType<ApplicationRejectionValidator>()
                   .As<IApplicationRejectionValidator>()
                   .SingleInstance();

            builder.RegisterType<ApplicationEventObserver>()
                   .As<IApplicationEventObserver>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<TaskEventObserver>()
                   .As<ITaskEventObserver>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<CollectionStepEventObserver>()
                   .As<ICollectionStepEventObserver>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<TriggerEventObserver>()
                   .As<ITriggerEventObserver>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<CheckEventObserver>()
                   .As<ICheckEventObserver>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<ReminderEventObserver>()
                   .As<IReminderEventObserver>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<ApplicationManager>()
                   .As<IApplicationManager>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<TaskManager>()
                   .As<ITaskManager>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<CollectionStepManager>()
                   .As<ICollectionStepManager>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<CheckManager>()
                   .As<ICheckManager>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<ReminderManager>()
                   .As<IReminderManager>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<TriggerManagerFactory>()
                   .As<ITriggerManagerFactory>()
                   .SingleInstance();

            builder.RegisterType<ApplicationApprovalService>()
                   .As<IApplicationApprovalService>()
                   .SingleInstance();

            builder.RegisterType<ApplicationRejectionService>()
                   .As<IApplicationRejectionService>()
                   .SingleInstance();

            builder.RegisterType<CheckCompletionService>()
                   .As<ICheckCompletionService>()
                   .SingleInstance();

            builder.RegisterType<HardcodedCheckOutputPolicyProvider>()
                   .As<ICheckOutputPolicyProvider>()
                   .SingleInstance();

            builder.RegisterType<TaskCompletionService>()
                   .As<ITaskCompletionService>()
                   .SingleInstance();

            builder.RegisterType<CheckSelectionService>()
                   .As<ICheckSelectionService>()
                   .SingleInstance();

            builder.RegisterType<CheckFilteringService>()
                   .As<ICheckFilteringService>()
                   .SingleInstance();

            builder.RegisterType<CheckOutputExtractionService>()
                   .As<ICheckOutputExtractionService>()
                   .SingleInstance();

            builder.RegisterType<ExternalProviderTypeMapper>()
                   .As<IExternalProviderTypeMapper>()
                   .SingleInstance();

            builder.RegisterType<UserReminderMapper>()
                   .As<IUserReminderMapper>()
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterConditions(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<ConditionServiceFactory>()
                   .As<IConditionServiceFactory>()
                   .As<ITriggerConditionServiceFactory>()
                   .SingleInstance();

            builder.RegisterType<ConditionsFactory>()
                   .As<IConditionsFactory>()
                   .SingleInstance();

            builder.RegisterConditionFactory<AccountAgeConditionFactory>(ConditionType.AccountAge)
                   .RegisterConditionFactory<IsPePConditionFactory>(ConditionType.IsPep)
                   .RegisterConditionFactory<MatchCountryConditionFactory>(ConditionType.MatchCountry)
                   .RegisterConditionFactory<TinTypeConditionFactory>(ConditionType.TinType)
                   .RegisterConditionFactory<RiskLevelConditionFactory>(ConditionType.RiskLevel)
                   .RegisterConditionFactory<RepeatingTurnoverConditionFactory>(ConditionType.RepeatingTurnover)
                   .RegisterConditionFactory<ExceededTurnoverConditionFactory>(ConditionType.ExceededTurnover)
                   .RegisterConditionFactory<MatchDecisionConditionFactory>(ConditionType.MatchDecision);

            return builder;
        }

        private static ContainerBuilder RegisterConditionFactory<TFactory>(this ContainerBuilder builder, ConditionType conditionType) where TFactory : IConditionFactory
        {
            builder.RegisterType<TFactory>().Keyed<IConditionFactory>(conditionType);
            return builder;
        }

        private static ContainerBuilder RegisterApplicationBuilder(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<ApplicationBuilder>()
                   .As<IApplicationBuilder>()
                   .SingleInstance();

            builder.RegisterType<ApplicationMigrationService>()
                   .As<IApplicationMigrationService>()
                   .SingleInstance();

            return builder;
        }

        private static void RegisterMonitoringServices(this ContainerBuilder builder)
        {
            builder.RegisterType<MonitoringPolicyProvider>()
                   .As<IMonitoringPolicyProvider>()
                   .SingleInstance();

            builder.RegisterDecorator<IMonitoringPolicyProvider>((context, _, provider) =>
            {
                var applicationStorage = context.Resolve<IApplicationStorage>();
                var hostSettingsProvider = context.Resolve<IHostSettingsProvider>();

                var environment = hostSettingsProvider.GetSetting(HostSettingsKey.Environment);
                var isProduction = environment.Equals("Production", StringComparison.InvariantCultureIgnoreCase);

                return isProduction ? provider : new MonitoringPolicyProviderDecorator(provider, applicationStorage);
            });

            builder.RegisterType<TriggerCommandRunner>()
                   .As<ITriggerCommandRunner>()
                   .SingleInstance();

            builder.RegisterType<CommandService>()
                   .As<ICommandService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);
        }

        private static void RegisterApprovalConditions(this ContainerBuilder builder)
        {
            builder.RegisterType<AllTasksPassedCondition>()
                   .As<IApplicationApprovalCondition>()
                   .SingleInstance();

            builder.RegisterType<RiskLevelEvaluatedCondition>()
                   .As<IApplicationApprovalCondition>()
                   .SingleInstance();

            builder.RegisterType<NoFiredTriggersCondition>()
                   .As<IApplicationApprovalCondition>()
                   .SingleInstance();
        }
    }
}
