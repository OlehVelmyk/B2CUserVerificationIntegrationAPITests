using System;
using System.Diagnostics;
using Autofac;
using Serilog;
using WX.B2C.User.Verification.Listener.Commands.IoC;
using WX.ServiceFabric.Autofac;

namespace WX.B2C.User.Verification.Listener.Commands
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
                ServiceRunner.StartedEvent += OnStartedEvent;
                ServiceRunner.InformationEvent += OnInformationEvent;
                ServiceRunner.FailedEvent += OnFailedEvent;
                ServiceRunner.Run<ListenerModule>();
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static void OnStartedEvent(object sender, ServiceRunner.StartedArgs args)
        {
            ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, ServiceDefinitions.ServiceTypeName);
            args.Container.Resolve<ILogger>().Information(args.Message);
        }

        private static void OnInformationEvent(object sender, string message)
        {
            ServiceEventSource.Current.Message(message);
        }

        private static void OnFailedEvent(object sender, ServiceRunner.FailedArgs e)
        {
            ServiceEventSource.Current.ServiceHostInitializationFailed($"{e.Error} EXCEPTION: {e.Exception}");
        }
    }
}
