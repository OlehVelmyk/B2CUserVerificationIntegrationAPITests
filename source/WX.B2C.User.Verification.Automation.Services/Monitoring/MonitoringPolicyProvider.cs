using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Monitoring;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Storages;

namespace WX.B2C.User.Verification.Automation.Services.Monitoring
{
    internal class MonitoringPolicyProvider : IMonitoringPolicyProvider
    {
        private readonly IPolicySelectionContextProvider _selectionContextProvider;
        private readonly IMonitoringPolicyStorage _policyStorage;

        public MonitoringPolicyProvider(IMonitoringPolicyStorage policyStorage,
                                        IPolicySelectionContextProvider selectionContextProvider)
        {
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _selectionContextProvider = selectionContextProvider ?? throw new ArgumentNullException(nameof(selectionContextProvider));
        }

        public async Task<MonitoringPolicyDto> FindAsync(Guid userId)
        {
            var selectionContext = await _selectionContextProvider.GetMonitoringContextAsync(userId);
            return await _policyStorage.FindAsync(selectionContext);
        }
    }
}