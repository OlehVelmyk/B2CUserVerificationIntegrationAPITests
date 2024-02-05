using WX.B2C.User.Verification.Infrastructure.ServiceFabric;

namespace WX.B2C.User.Verification.Api.Public
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            StatelessServiceRunner.Run<PublicApiService>(ServiceDefinitions.ServiceTypeName, ServiceEventSource.Current);
        }
    }
}
