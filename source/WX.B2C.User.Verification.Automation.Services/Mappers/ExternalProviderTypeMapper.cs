using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal interface IExternalProviderTypeMapper
    {
        ExternalProviderType? Map(CheckProviderType providerType);
    }

    internal class ExternalProviderTypeMapper : IExternalProviderTypeMapper
    {
        public ExternalProviderType? Map(CheckProviderType providerType)
        {
            return providerType switch
            {
                CheckProviderType.Onfido => ExternalProviderType.Onfido,
                CheckProviderType.PassFort => ExternalProviderType.PassFort,
                CheckProviderType.LexisNexis => null,
                CheckProviderType.System => null,
                _ => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, "Unsupported check provider type.")
            };
        }
    }
}