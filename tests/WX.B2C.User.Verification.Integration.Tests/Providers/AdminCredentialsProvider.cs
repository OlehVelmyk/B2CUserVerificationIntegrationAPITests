using System.IdentityModel.Tokens.Jwt;
using WX.Configuration.Admin.Interface;
using WX.Configuration.Contracts.Tests.Interface;
using WX.Integration.Tests.HttpHandlers.AdminAuth;
using WX.KeyVault;

namespace WX.B2C.User.Verification.Integration.Tests.Providers;

internal class AdminCredentialsProvider
{
    private readonly IAdminConfiguration _adminConfiguration;
    private readonly IAdminLoginInformation _adminLoginInformation;
    private readonly KeyVaultConfiguration _keyVaultConfiguration;
    private string? _token;
    private string? _tokenType;

    public AdminCredentialsProvider(IAdminConfiguration adminConfiguration,
                                    IAdminLoginInformation adminLoginInformation,
                                    KeyVaultConfiguration keyVaultConfiguration)
    {
        _adminConfiguration = adminConfiguration;
        _adminLoginInformation = adminLoginInformation;
        _keyVaultConfiguration = keyVaultConfiguration;
    }

    public async Task<(string token, string tokenType)> GetAsync()
    {
        if (_token is null || _tokenType is null)
            (_token, _tokenType) = await GenerateToken();
        
        if (IsTokenExpired())
            (_token, _tokenType) = await GenerateToken();
        
        return (_token, _tokenType);
    }

    private bool IsTokenExpired()
    {
        var jwtToken = new JwtSecurityToken(_token);
        return jwtToken.ValidFrom > DateTime.UtcNow || jwtToken.ValidTo < DateTime.UtcNow;
    }

    private async Task<(string token, string tokenType)> GenerateToken()
    {
        var httpClient = new HttpClient();
        var response = await OAuthHelper.GetToken(_adminConfiguration, _keyVaultConfiguration, _adminLoginInformation, httpClient);
        return (response.AccessToken, response.TokenType);
    }
}
