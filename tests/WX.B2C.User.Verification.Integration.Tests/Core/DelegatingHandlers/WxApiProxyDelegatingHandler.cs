using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;

namespace WX.B2C.User.Verification.Integration.Tests.Core.DelegatingHandlers;

internal class WxApiAuthenticationDelegatingHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", StepContext.Instance[General.AccessToken]);
        
        request.RequestUri = GetWxApiUri(request.RequestUri!.ToString());

        return base.SendAsync(request, cancellationToken);
    }

    private Uri GetWxApiUri(string original)
    {
        foreach (var replace in Replacements())
            original = replace(original);

        return new Uri(original);
    }

    private IEnumerable<Func<string, string>> Replacements()
    {
        yield return original => Regex.Replace(original, @"api/v\d+/users/surveys/", @"/service/surveys/");
        yield return original => Regex.Replace(original, @"api/v\d+/verification", @"service/verification");
        yield return original => Regex.Replace(original, @"/user/profile", @"/service/user/profile");
    }
}
