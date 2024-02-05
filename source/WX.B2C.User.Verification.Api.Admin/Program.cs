using WX.B2C.User.Verification.Infrastructure.ServiceFabric;

namespace WX.B2C.User.Verification.Api.Admin
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            StatelessServiceRunner.Run<AdminApiService>(ServiceDefinitions.ServiceTypeName, ServiceEventSource.Current);
        }
    }
}
