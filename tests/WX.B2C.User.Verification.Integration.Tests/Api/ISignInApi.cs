using Newtonsoft.Json;
using RestEase;

namespace WX.B2C.User.Verification.Integration.Tests.Api;

[Header("Accept", "application/json")]
[Header("Content-Type", "application/x-www-form-urlencoded")]
[SerializationMethods(Body = BodySerializationMethod.UrlEncoded)]
public interface ISignInApi
{
    [Post("/v6/signin")]
    Task<SignInResponse> SignIn([Body] Dictionary<string, object> request);
}

public class SignInResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
    
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
}