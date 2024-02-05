using System;
using System.Threading;
using Autofac;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using WX.B2C.User.Verification.Actor.Actors;
using WX.B2C.User.Verification.Actor.IoC;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Triggers;

namespace WX.B2C.User.Verification.Actor
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                var serviceCollection = new ContainerBuilder();
                serviceCollection.RegisterModule<ActorModule>();
                var container = serviceCollection.Build();

                RegisterActor(VerificationActorFactory);
                RegisterActor(ProfileActorFactory);
                RegisterActor(TaskActorFactory);
                RegisterActor(CollectionStepActorFactory);
                RegisterActor(CheckActorFactory);
                RegisterActor(ExternalProfileActorFactory);
                RegisterActor(TriggerActorFactory);

                ApplicationActor VerificationActorFactory(ActorService service, ActorId id) =>
                    new ApplicationActor(service, id, container.Resolve<IApplicationService>());

                ProfileActor ProfileActorFactory(ActorService service, ActorId id) =>
                    new ProfileActor(service, id, 
                        container.Resolve<IProfileService>(), 
                        container.Resolve<IDocumentService>(), 
                        container.Resolve<IFileService>());

                TaskActor TaskActorFactory(ActorService service, ActorId id) =>
                    new TaskActor(service, id, container.Resolve<ITaskService>());                
                
                CollectionStepActor CollectionStepActorFactory(ActorService service, ActorId id) =>
                    new CollectionStepActor(service, id, container.Resolve<ICollectionStepService>());                
                
                CheckActor CheckActorFactory(ActorService service, ActorId id) =>
                    new CheckActor(service, id, container.Resolve<ICheckService>());

                ExternalProfileActor ExternalProfileActorFactory(ActorService service, ActorId id) =>
                    new ExternalProfileActor(service, id, container.Resolve<IExternalProfileProvider>());

                TriggerActor TriggerActorFactory(ActorService service, ActorId id) =>
                    new TriggerActor(service, id, container.Resolve<ITriggerService>());

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
            }
        }

        private static void RegisterActor<T>(Func<ActorService, ActorId, T> actorFactory) where T : ActorBase
        {
            ActorRuntime.RegisterActorAsync<T>((context, actorType) =>
                                                   new ActorService(context, actorType, actorFactory))
                        .GetAwaiter()
                        .GetResult();
        }
    }
}