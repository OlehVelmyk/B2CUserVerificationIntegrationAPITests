using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Automation.Services.Extensions;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services
{
    public interface IApplicationRejectionService
    {
        Task<ApplicationDto[]> FindApplicationsToRejectAsync(IReadOnlyCollection<ApplicationDto> applications, string[] changes);
    }

    internal class ApplicationRejectionService : IApplicationRejectionService
    {
        private readonly IVerificationPolicyStorage _policyStorage;
        private readonly IConditionServiceFactory _conditionServiceFactory;

        public ApplicationRejectionService(IVerificationPolicyStorage policyStorage, IConditionServiceFactory conditionServiceFactory)
        {
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _conditionServiceFactory = conditionServiceFactory ?? throw new ArgumentNullException(nameof(conditionServiceFactory));
        }

        public async Task<ApplicationDto[]> FindApplicationsToRejectAsync(IReadOnlyCollection<ApplicationDto> applications, string[] changes)
        {
            var userId = applications.Select(x => x.UserId).FirstOrDefault();

            if (applications.Any(dto => dto.UserId != userId))
                throw new ArgumentException($"Provided applications not related to user {userId}");

            var appliedApplications = applications.Where(application => application.CanBeRejected()).ToArray();
            if (appliedApplications.Length == 0)
                return Array.Empty<ApplicationDto>();

            var policies = await _policyStorage.GetAsync(appliedApplications.Select(dto => dto.PolicyId));

            var policiesWithRejectPolicy = policies.Where(dto => dto.RejectionPolicy != null);
            var applicationsToReject = (await GetApplicationsToRejectAsync(userId, policiesWithRejectPolicy, changes))
                                       .Select(dto => dto.Id)
                                       .ToArray();

            return appliedApplications.Where(application => applicationsToReject.Contains(application.PolicyId)).ToArray();
        }

        private Task<IEnumerable<VerificationPolicyDto>> GetApplicationsToRejectAsync(Guid userId, IEnumerable<VerificationPolicyDto> policies, string[] changes)
        {
            var conditionService = _conditionServiceFactory.Create(userId);

            return policies.Where(dto => MustRejectAsync(dto, conditionService, changes));
        }

        private Task<bool> MustRejectAsync(VerificationPolicyDto policy, IConditionService conditionService, string[] changes)
        {
            var conditions = policy.RejectionPolicy.Conditions;
            var dependentConditions = conditions
                                      .Where(condition => conditionService.GetRequiredDataXPath(condition.Type).Intersect(changes).Any())
                                      .ToArray();

            return conditionService.IsAnySatisfiedAsync(dependentConditions);
        }
    }
}