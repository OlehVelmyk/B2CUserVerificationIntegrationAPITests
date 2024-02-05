using WX.B2C.User.Verification.Infrastructure.ServiceFabric;
using WX.B2C.User.Verification.Api.Webhook.Services;

namespace WX.B2C.User.Verification.Api.Webhook
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            StatelessServiceRunner.Run<WebhookService>(ServiceDefinitions.ServiceTypeName, ServiceEventSource.Current);
        }
    }
}