namespace WX.B2C.User.Verification.Infrastructure.Remoting
{
    public sealed class Constants
    {
        public sealed class Headers
        {
            public const string OperationName = nameof(OperationName);
            public const string ParentOperationId = nameof(ParentOperationId);
            public const string OperationId = nameof(OperationId);
            public const string CorrelationId = nameof(CorrelationId);
            public const string Reference = nameof(Reference);
            public const string ProvidedEmptyCorrelation = nameof(ProvidedEmptyCorrelation);
        }
    }
}
