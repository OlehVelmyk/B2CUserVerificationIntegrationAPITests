namespace WX.B2C.User.Verification.Infrastructure.Contracts
{
    public interface IEncryptProvider
    {
        string EncryptText(string data);

        string DecryptText(string data);
    }
}