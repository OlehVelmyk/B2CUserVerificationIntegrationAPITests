using System;
using Autofac;
using FluentValidation;
using WX.B2C.User.Verification.Facade.EventHandlers.Mappers;
using WX.B2C.User.Verification.Facade.EventHandlers.Validators;
using WX.Messaging.Subscriber.Autofac;

namespace WX.B2C.User.Verification.Facade.EventHandlers.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterEventHandlers(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<EventHandlingContext>().InstancePerLifetimeScope();
             
            return builder
                   .RegisterMappers()
                   .RegisterValidators()
                   .RegisterHandlers();
        }

        private static ContainerBuilder RegisterHandlers(this ContainerBuilder builder) => 
               builder.RegisterEventHandler<AuditEventHandler>()
                      .RegisterEventHandler<CheckEventHandler>()
                      .RegisterEventHandler<CollectionStepEventHandler>()
                      .RegisterEventHandler<RiskLevelEventHandler>()
                      .RegisterEventHandler<ApplicationEventHandler>()
                      .RegisterEventHandler<ApplicationStateChangelogEventHandler>()
                      .RegisterEventHandler<UserResourcesChangedNotificationsEventHandler>()
                      .RegisterEventHandler<UserTextNotificationEventHandler>()
                      .RegisterEventHandler<UserProfileEventHandler>()
                      .RegisterEventHandler<CollectionStepEventHandler>()
                      .RegisterEventHandler<TicketEventHandler>()
                      .RegisterEventHandler<TaskEventHandler>()
                      .RegisterEventHandler<TriggersEventHandler>()
                      .RegisterEventHandler<TriggerCommandsEventHandler>()
                      .RegisterEventHandler<JobEventHandler>()
                      .RegisterEventHandler<TurnoverEventHandler>()
                      .RegisterEventHandler<FileEventHandler>()
                      .RegisterEventHandler<PolicyEventHandler>()
                      .RegisterEventHandler<DocumentEventHandler>()
                      .RegisterEventHandler<UserEmailEventHandler>()
                      .RegisterEventHandler<UserReminderEventHandler>();

        private static ContainerBuilder RegisterMappers(this ContainerBuilder builder)
        {
            builder.RegisterType<AuditDataSerializer>()
                   .As<IAuditDataSerializer>()
                   .SingleInstance();

            builder.RegisterType<ProfileDetailsMapper>()
                   .As<IProfileDetailsMapper>()
                   .SingleInstance();

            builder.RegisterType<ResourceNotificationMapper>()
                   .As<IResourceNotificationMapper>()
                   .SingleInstance();

            builder.RegisterType<TextNotificationMapper>()
                   .As<ITextNotificationMapper>()
                   .SingleInstance();

            builder.RegisterType<CheckTicketMapper>()
                   .As<ICompletedCheckMapper>()
                   .SingleInstance();

            builder.RegisterType<AuditModelsMapper>()
                   .As<IAuditModelsMapper>()
                   .SingleInstance();

            builder.RegisterType<InitiationMapper>()
                   .As<IInitiationMapper>()
                   .SingleInstance();

            builder.RegisterType<JobRequestMapper>()
                   .As<IJobRequestMapper>()
                   .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterValidators(this ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(AddressValidator).Assembly)
                   .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                   .AsImplementedInterfaces();

            return builder;
        }
    }
}