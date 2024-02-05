using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Steps;

namespace WX.B2C.User.Verification.Integration.Tests.Extensions;

internal static class StepExtensions
{
    public static CreateApplicationStep WithIpAddress(this CreateApplicationStep step, string ipAddress) =>
        step.WithCustomHeaders(new Dictionary<string, List<string>>
        {
            { Headers.TestIpAddress, new List<string> { ipAddress } }
        });
}
