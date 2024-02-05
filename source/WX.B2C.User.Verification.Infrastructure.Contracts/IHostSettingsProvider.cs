namespace WX.B2C.User.Verification.Infrastructure.Contracts
{
    public interface IHostSettingsProvider
    {
        string GetSetting(string name);
    }
}
