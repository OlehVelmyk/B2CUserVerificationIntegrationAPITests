using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using OutputCheckDto = WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos.CheckDto;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Services
{
    public interface ICheckAggregationService
    {
        Task<OutputCheckDto> AggregateAsync(CheckDto check);

        Task<IEnumerable<OutputCheckDto>> AggregateAsync(CheckDto[] checks);
    }

    internal class CheckAggregationService : ICheckAggregationService
    {
        private readonly IVerificationPolicyStorage _verificationPolicyStorage;
        private readonly ICheckStorage _checkStorage;
        private readonly ICheckMapper _mapper;

        public CheckAggregationService(
            IVerificationPolicyStorage verificationPolicyStorage, 
            ICheckStorage checkStorage,
            ICheckMapper mapper)
        {
            _verificationPolicyStorage = verificationPolicyStorage ?? throw new ArgumentNullException(nameof(verificationPolicyStorage));
            _checkStorage = checkStorage ?? throw new ArgumentNullException(nameof(checkStorage));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<OutputCheckDto> AggregateAsync(CheckDto check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            var result = await AggregateAsync(new[] { check });
            return result.Single();
        }

        public async Task<IEnumerable<OutputCheckDto>> AggregateAsync(CheckDto[] checks)
        {
            if (checks == null)
                throw new ArgumentNullException(nameof(checks));
            if (!checks.Any())
                return Enumerable.Empty<OutputCheckDto>();

            var checkVariants = await GetCheckVariantsAsync(checks);
            var relatedTasksMap = await GetRelatedTasksAsync(checks);
            var mappedChecks = await checks.Select(check => Map(check, checkVariants, relatedTasksMap)).WhenAll();
            return mappedChecks.OrderByDescending(check => check.CreatedAt);
        }

        private Task<VariantNameDto[]> GetCheckVariantsAsync(IEnumerable<CheckDto> checks)
        {
            var checkVariantIds = checks.Select(c => c.Variant.Id)
                                        .Distinct()
                                        .ToArray();

            return _verificationPolicyStorage.GetCheckVariantNamesAsync(checkVariantIds);
        }

        private async Task<Dictionary<Guid, Guid[]>> GetRelatedTasksAsync(IEnumerable<CheckDto> checks)
        {
            var checkIds = checks.Select(check => check.Id);
            var relatedTasks = await checkIds.Select(GetRelatedTasks).WhenAll();

            return relatedTasks.ToDictionary(
                check => check.Id, 
                check => check.RelatedTasks);

            Task<(Guid Id, Guid[] RelatedTasks)> GetRelatedTasks(Guid checkId) =>
                _checkStorage.GetRelatedTasksAsync(checkId)
                             .Select(taskIds => (checkId, taskIds));
        }

        private async Task<OutputCheckDto> Map(CheckDto check, 
                                               IEnumerable<VariantNameDto> checkVariants, 
                                               IDictionary<Guid, Guid[]> relatedTasksMap)
        {
            var checkVariant = checkVariants.FirstOrDefault(x => x.Id == check.Variant.Id);
            var relatedTasks = relatedTasksMap[check.Id];
            return await _mapper.Map(check, checkVariant, relatedTasks);
        }
    }
}
