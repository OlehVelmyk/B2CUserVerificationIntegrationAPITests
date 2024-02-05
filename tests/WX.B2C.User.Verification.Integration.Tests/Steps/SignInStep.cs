using System.Security.Cryptography;
using System.Text;
using WX.B2C.User.Verification.Integration.Tests.Api;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;

namespace WX.B2C.User.Verification.Integration.Tests.Steps;

internal sealed class SignInStep : BaseStep
{
    private readonly ISignInApi _signInApi;

    public SignInStep(ISignInApi signInApi)
    {
        _signInApi = signInApi;
    }

    public override Task Execute()
    {
        return SignIn();
    }

    private async Task SignIn()
    {
        var signature = GenerateSignature(
            StepContext.Instance[General.UserName],
            StepContext.Instance[General.SignInClientId],
            StepContext.Instance[General.SignInClientSecret],
            StepContext.Instance[General.SignInDeviceId],
            StepContext.Instance[General.SignInDeviceName]);

        var request = new Dictionary<string, object> {
            {"grant_type", "password"},
            {"client_id", StepContext.Instance[General.SignInClientId] },
            {"username", StepContext.Instance[General.UserName] },
            {"password", StepContext.Instance[General.Password] },
            {"device_id", StepContext.Instance[General.SignInDeviceId] },
            {"device_name", StepContext.Instance[General.SignInDeviceName] },
            {"signature", signature },
            {"client_secret", StepContext.Instance[General.SignInClientSecret] },
        };

        var result = await _signInApi.SignIn(request);

        StepContext.Instance[General.AccessToken] = result.AccessToken;
    }

    private static string GenerateSignature(string login,
        string clientId,
        string clientSecret,
        string deviceId,
        string deviceName)
    {
        var data = clientId + login + deviceId + deviceName;

        using var hmacsha512 = new HMACSHA512(Encoding.UTF8.GetBytes(clientSecret));
        var hash = hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }
}
