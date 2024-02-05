using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.PassFort.Client;
using WX.B2C.User.Verification.PassFort.Client.Models;
using WX.B2C.User.Verification.PassFort.Configuration;
using WX.B2C.User.Verification.PassFort.Exceptions;
using WX.B2C.User.Verification.PassFort.Mappers;
using WX.B2C.User.Verification.PassFort.Models;
using WX.B2C.User.Verification.PassFort.Processors;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Extensions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using ErrorCodes = WX.B2C.User.Verification.Provider.Contracts.Constants.ErrorCodes;

namespace WX.B2C.User.Verification.PassFort.Runners
{
    internal sealed class RiskScreeningCheckRunner : AsyncCheckRunner<RiskScreeningCheckData>
    {
        private readonly RiskScreeningCheckConfiguration _configuration;
        private readonly IPassFortApiClientFactory _clientFactory;
        private readonly IPassFortProfileUpdater _profileUpdater;
        private readonly ICheckProcessor _checkProcessor;
        private readonly IProfileDataPatchMapper _profileDataPatchMapper;

        public RiskScreeningCheckRunner(
            RiskScreeningCheckConfiguration configuration,
            IPassFortApiClientFactory clientFactory,
            ICheckProcessor checkProcessor,
            IPassFortProfileUpdater profileUpdater,
            IProfileDataPatchMapper profileDataPatchMapper)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _profileUpdater = profileUpdater ?? throw new ArgumentNullException(nameof(profileUpdater));
            _checkProcessor = checkProcessor ?? throw new ArgumentNullException(nameof(checkProcessor));
            _profileDataPatchMapper = profileDataPatchMapper ?? throw new ArgumentNullException(nameof(profileDataPatchMapper));
        }

        public override async Task<CheckRunningResult> RunAsync(RiskScreeningCheckData checkData, CancellationToken cancellationToken = default)
        {
            try
            {
                var profileId = checkData.ProfileId;

                var profilePatch = _profileDataPatchMapper.Map(checkData);
                await _profileUpdater.UpdateAsync(profileId, profilePatch);

                var client = _clientFactory.Create();

                var associatedTaskIds = await GetAssociatedTaskIdsAsync(client, profileId);

                var request = new CheckRequest
                {
                    CheckType = Constants.CheckTypes.PepAndSanctionsScreen,
                    TaskIds = associatedTaskIds.ToArray()
                };

                var response = await client.Checks.RunAsync(profileId, request, cancellationToken: cancellationToken);
                var additionalData = new Dictionary<string, object> { [nameof(RiskScreeningCheckData.ProfileId)] = profileId };
                return CheckRunningResult.Instructed(response.Id, additionalData);
            }
            catch (ValidationException exc)
            {
                throw new CheckExecutionException(ErrorCodes.InvalidInputData, exc.Message);
            }
            catch (HttpOperationException exc)
            {
                throw new CheckExecutionException(ErrorCodes.ProviderUnknownError, exc.Response?.Content);
            }
            catch (PassFortApiException exc)
            {
                throw new CheckExecutionException(ErrorCodes.ProviderUnknownError, exc.Message);
            }
        }

        public override async Task<CheckProcessingResult> GetResultAsync(CheckProcessingContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var checkId = context.ExternalData.GetExternalId();
                var profileId = context.ExternalData.Get<string>(nameof(RiskScreeningCheckData.ProfileId));

                var client = _clientFactory.Create();

                var check = await client.Checks.GetAsync(profileId, checkId, cancellationToken);

                if (check is not CheckResourcePepsAndSanctionsScreen riskScreeningCheck)
                    throw new CheckProcessingException(ErrorCodes.ProcessingError, "Unexpected check type.");

                return _checkProcessor.Process(riskScreeningCheck);
            }
            catch (ValidationException exc)
            {
                throw new CheckProcessingException(ErrorCodes.InvalidInputData, exc.Message);
            }
            catch (HttpOperationException exc)
            {
                throw new CheckProcessingException(ErrorCodes.ProviderUnknownError, exc.Body?.ToString());
            }
            catch (PassFortApiException exc)
            {
                throw new CheckProcessingException(ErrorCodes.ProviderUnknownError, exc.Message);
            }
        }

        private async Task<IEnumerable<string>> GetAssociatedTaskIdsAsync(IPassFortApiClient client, string profileId)
        {
            var taskTypes = GetRiskScreeningTaskTypes();

            var allTasks = await client.Tasks.ListAsync(profileId);
            var riskScreeningTasks = allTasks
                .Where(IsActive)
                .Where(IsRiskScreeningTask)
                .ToList();

            var missingTasks = GetMissingTasks(taskTypes, riskScreeningTasks).ToArray();
            if (missingTasks.Any())
            {
                var createdTasks = await missingTasks.Select(CreateTask).WhenAll();
                riskScreeningTasks.AddRange(createdTasks);
            }

            return riskScreeningTasks.Select(task => task.Id);

            static bool IsActive(TaskResource task) => task is { IsExpired: false };
            bool IsRiskScreeningTask(TaskResource task) => task.Type.HasValue && task.Type.Value.In(taskTypes);
            Task<TaskResource> CreateTask(TaskType taskType) => CreateTaskAsync(client, profileId, taskType);
        }

        private static IEnumerable<TaskType> GetMissingTasks(IEnumerable<TaskType> taskTypes, IEnumerable<TaskResource> tasks)
        {
            return taskTypes
                   .GroupJoin(tasks,
                       type => type,
                       task => task.Type,
                       (type, matchedTasks) => new
                       {
                           Type = type, 
                           ShouldCreate = !matchedTasks.Any()
                       })
                   .Where(task => task.ShouldCreate)
                   .Select(task => task.Type)
                   .ToArray();   
        }

        private static Task<TaskResource> CreateTaskAsync(IPassFortApiClient client, string profileId, TaskType taskType)
        {
            var request = new TaskRequest { Type = taskType };
            return client.Tasks.AddAsync(profileId, request);
        }

        private TaskType[] GetRiskScreeningTaskTypes()
        {
            var useAdverseMediaFlow = _configuration.UseAdverseMediaFlow;
            if (!useAdverseMediaFlow)
            {
                return new[]
                {
                    TaskType.INDIVIDUALASSESSMEDIAANDPOLITICALANDSANCTIONSEXPOSURE
                };
            }

            return new[]
            {
                TaskType.INDIVIDUALASSESSPOLITICALEXPOSURE,
                TaskType.INDIVIDUALASSESSSANCTIONSEXPOSURE
            };
        }
    }
}
