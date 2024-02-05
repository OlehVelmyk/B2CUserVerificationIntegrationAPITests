using WX.B2C.User.Verification.Integration.Tests.Constants;

namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure;

internal class StepContext
{
    private readonly Dictionary<object, string> _contextItems = new();

    public string this[object item]
    {
        get => (_contextItems.ContainsKey(item) ? _contextItems[item] : null) ?? string.Empty;

        set => _contextItems[item] = value;
    }

    public static StepContext Instance { get; private set; } = new();

    public static void FillSignUpSignInData(UserSection userSettings)
    {
        Instance[General.SignUpClientId] = userSettings.ClientId;
        Instance[General.SignUpClientSecret] = userSettings.ClientSecret;
        Instance[General.SignUpDeviceId] = userSettings.DeviceId;
        Instance[General.SignUpDeviceName] = userSettings.DeviceName;

        Instance[General.SignInClientId] = userSettings.ClientId;
        Instance[General.SignInClientSecret] = userSettings.ClientSecret;
        Instance[General.SignInDeviceId] = userSettings.DeviceId;
        Instance[General.SignInDeviceName] = userSettings.DeviceName;
    }

    public static void Reset()
    {
        Instance = new StepContext();
    }
}
