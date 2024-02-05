namespace WX.B2C.User.Verification.Infrastructure.Contracts
{
    public interface IOperationContextProvider
    {
        public OperationContext GetContextOrDefault();
    }
}
