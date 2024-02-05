using System;
using System.Diagnostics;
using Autofac;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using WX.ServiceFabric.Autofac;

namespace WX.B2C.User.Verification.Infrastructure.ServiceFabric
{
    public static class StatelessServiceRunner
    {
        public static void Run<T>(string serviceTypeName, IEventSource eventSource) where T : StatelessService
        {
            try
            {
                StatelessServiceModule<T>.Setup(serviceTypeName, eventSource);
                ServiceRunner.StartedEvent += (_, args) =>
                {
                    eventSource.ServiceTypeRegistered(Process.GetCurrentProcess().Id, serviceTypeName);
                    args.Container.Resolve<ILogger>().Information(args.Message);
                };
                ServiceRunner.InformationEvent += (_, message) =>
                {
                    eventSource.Message(message);
                };
                ServiceRunner.FailedEvent += (_, args) =>
                {
                    eventSource.ServiceHostInitializationFailed($"{args.Error} EXCEPTION: {args.Exception}");
                };
                ServiceRunner.Run<StatelessServiceModule<T>>();
            }
            catch (Exception e)
            {
                eventSource.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}