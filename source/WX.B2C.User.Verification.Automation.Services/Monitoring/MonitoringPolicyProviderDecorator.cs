using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Monitoring;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Monitoring
{
    internal class MonitoringPolicyProviderDecorator : IMonitoringPolicyProvider
    {
        private readonly IMonitoringPolicyProvider _inner;
        private readonly IApplicationStorage _applicationStorage;
        // If amount of policies will increase actively then extract them to options and configurations
        private readonly Guid[] _simplifiedPolicies =
        {
            new("798D88FE-A49D-412B-94F3-845BA43969B9")
        };

        public MonitoringPolicyProviderDecorator(IMonitoringPolicyProvider inner, IApplicationStorage applicationStorage)
        {
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public async Task<MonitoringPolicyDto> FindAsync(Guid userId)
        {
            var application = await _applicationStorage.FindAsync(userId, ProductType.WirexBasic);
            if (application?.PolicyId.In(_simplifiedPolicies) ?? false)
                return null;

            return await _inner.FindAsync(userId);
        }
    }
}
