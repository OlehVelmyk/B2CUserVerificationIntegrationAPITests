namespace WX.B2C.User.Verification.Infrastructure.ServiceFabric
{
    public interface IEventSource
    {
        void ServiceHostInitializationFailed(string exception);

        void ServiceTypeRegistered(int id, string serviceTypeName);

        void Message(string message);
    }
}