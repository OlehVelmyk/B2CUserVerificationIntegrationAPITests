namespace WX.B2C.User.Verification.Infrastructure.Common.Monitoring
{
    public static class OperationProtocols
    {
        public const string RPC = nameof(RPC);
        public const string HTTP = nameof(HTTP);
        public const string Event = nameof(Event);
        public const string Command = nameof(Command);
        public const string InMemory = nameof(InMemory);
    }
}