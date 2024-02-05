using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Provider.Contracts.Extensions
{
    public static class ExternalDataExtensions
    {
        public static string GetExternalId(this IReadOnlyDictionary<string, object> externalData) =>
            externalData.Get<string>(ExternalCheckProperties.ExternalId);
    }
}
