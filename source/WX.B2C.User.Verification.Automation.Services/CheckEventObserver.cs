using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Automation.Services.Extensions;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal class CheckEventObserver : ICheckEventObserver
    {
        private readonly ICheckStorage _checkStorage;
        private readonly ICheckManager _checkManager;
        private readonly ICheckSelectionService _checkSelectionService;
        private readonly ICheckCompletionService _completionService;

        public CheckEventObserver(
            ICheckStorage checkStorage,
            ICheckManager checkManager,
            ICheckSelectionService checkSelectionService,
            ICheckCompletionService completionService)
        {
            _checkStorage = checkStorage ?? throw new ArgumentNullException(nameof(checkStorage));
            _checkManager = checkManager ?? throw new ArgumentNullException(nameof(checkManager));
            _checkSelectionService = checkSelectionService ?? throw new ArgumentNullException(nameof(checkSelectionService));
            _completionService = completionService ?? throw new ArgumentNullException(nameof(completionService));
        }

        public Task OnProfileChanged(Guid userId, string[] xPathes)
        {
            // Try to re-instruct checks which was already performed but are affected by new profile changes.

            // 1) Find check variants which could be affected by changes and should be re-run
            // 2) Apply run policy to validate that check could be re-run
            // 3) Validate that there are no pending checks to prevent running on the same input data
            // 4) Send 'request check' command for each discovered check variant

            var reason = "Profile data has changed.";
            return TryInstructAsync(userId, xPathes, reason);
        }

        public async Task OnCollectionStepRequested(Guid userId, string xPath)
        {
            // Try to re-instruct checks which was already performed but admin requested new collection step.

            // 1) Find check variants which could be dependent on requested collection step and should be re-run
            // 2) Apply run policy to validate that check could be re-run
            // 3) Validate that there are no pending checks to prevent running on the same input data
            // 4) Send 'request check' command for each discovered check variant

            var reason = "New collection step requested.";
            await TryInstructAsync(userId, new[] { xPath }, reason);
        }

        public async Task OnCollectionStepCompleted(Guid userId, string xPath)
        {
            // Try to run checks which is in pending because collection step completed.

            // 1) Find pending user checks (they are pending for two reasons: collection steps is not completed or provider is not available)
            // 2) Filter checks which is not ready to run yet 
            // 3) Select provider on which check should be run (for waterfall feature)
            // 4) Prepare check input data
            // 5) Build CheckRunningContext
            // 6) Send 'run check' command to CheckProvider

            var pendingChecks = await _checkSelectionService.GetPendingChecksAsync(userId);
            var affectedChecks = pendingChecks
                                 .Where(check => check.Parameters.MatchesXPath(xPath) || check.ShouldBeAggregated)
                                 .ToArray();

            await _checkManager.TryRunAsync(userId, affectedChecks);
        }

        public async Task OnCheckCreated(Guid userId, Guid checkId)
        {
            // Try run check as soon as check created because all collection steps could be already completed.

            var pendingCheck = await _checkSelectionService.GetPendingCheckAsync(checkId);
            var affectedChecks = new[] { pendingCheck };
            await _checkManager.TryRunAsync(userId, affectedChecks);
        }

        /// <summary>
        /// Is needed for massive re-run checks from job
        /// </summary>
        public async Task OnChecksCreated(Guid userId, Guid[] checks)
        {
            var pendingChecks = await _checkSelectionService.GetPendingChecksAsync(userId);
            var affectedChecks = pendingChecks
                                 .Where(check => check.Id.In(checks) || check.ShouldBeAggregated)
                                 .ToArray();

            await _checkManager.TryRunAsync(userId, affectedChecks);
        }

        public async Task OnCheckPerformed(Guid checkId)
        {
            // Webhook that external check completed has been received and check result processing should be started.

            var check = await _checkStorage.GetAsync(checkId);
            await _checkManager.TryProcessAsync(check);
        }

        public async Task OnCheckPassed(Guid checkId)
        {
            var checkDto = await _checkStorage.GetAsync(checkId);
            await _completionService.TryExtractDataAsync(checkDto);
        }

        public async Task OnCheckFailed(Guid checkId)
        {
            var checkDto = await _checkStorage.GetAsync(checkId);
            await Task.WhenAll(
                _completionService.TryApplyFailPolicyAsync(checkDto),
                _completionService.TryExtractDataAsync(checkDto));
        }

        public async Task OnTaskCompleted(Guid userId)
        {
            var pendingChecks = await _checkSelectionService.GetPendingChecksAsync(userId);
            await _checkManager.TryRunAsync(userId, pendingChecks);
        }

        /// <summary>
        /// TODO WRXB-10546 remove in phase 2 when all users finally migrated
        /// </summary>
        public async Task OnApplicationAutomatedAsync(Guid userId)
        {
            var pendingChecks = await _checkSelectionService.GetPendingChecksAsync(userId);
            await _checkManager.TryRunAsync(userId, pendingChecks);
        }

        private async Task TryInstructAsync(Guid userId, string[] xPathes, string reason)
        {
            var acceptanceChecks = await _checkSelectionService.GetAcceptanceChecksAsync(userId);
            var affectedChecks = acceptanceChecks.Where(check => check.Parameters.MatchesXPath(xPathes));
            await _checkManager.TryInstructAsync(userId, affectedChecks, reason);
        }
    }
}
