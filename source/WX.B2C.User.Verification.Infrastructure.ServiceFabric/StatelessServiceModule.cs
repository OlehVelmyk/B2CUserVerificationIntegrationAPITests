using System;
using Autofac;
using Autofac.Integration.ServiceFabric;
using Microsoft.ServiceFabric.Services.Runtime;
using WX.B2C.User.Verification.Infrastructure.Common.IoC;

namespace WX.B2C.User.Verification.Infrastructure.ServiceFabric
{
    internal class StatelessServiceModule<T> : Module where T : StatelessService
    {
        private static string ServiceTypeName;
        private static IEventSource EventSource;

        public static void Setup(string serviceTypeName, IEventSource eventSource)
        {
            if (ServiceTypeName != null || EventSource != null)
                throw new InvalidOperationException("Stateless service run parameters was already set");

            ServiceTypeName = serviceTypeName ?? throw new ArgumentNullException(nameof(serviceTypeName));
            EventSource = eventSource ?? throw new ArgumentNullException(nameof(eventSource));
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterServiceRunningInfrastructure();
            builder.RegisterServiceFabricSupport(OnException);
            builder.RegisterStatelessService<T>(ServiceTypeName);
        }

        private static void OnException(Exception exception) =>
            EventSource.ServiceHostInitializationFailed(exception.ToString());
    }
}
