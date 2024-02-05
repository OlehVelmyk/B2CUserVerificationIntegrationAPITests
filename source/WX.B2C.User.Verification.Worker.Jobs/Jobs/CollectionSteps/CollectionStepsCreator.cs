using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using ICollectionStepRepository = WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories.ICollectionStepRepository;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal interface ICollectionStepsCreator
    {
        Task CreateStepsAsync(Guid userId,
                              Guid policyId,
                              Dictionary<string, NewCollectionStep> expectedSteps,
                              CollectionStepsCreator.TaskInfo[] tasks);
    }

    internal class CollectionStepsCreator : ICollectionStepsCreator
    {
        private readonly ICollectionStepsPolicyProvider _collectionStepsPolicyProvider;
        
        private readonly ICollectionStepRepository _collectionStepRepository;
        private readonly ILogger _logger;
        
        private readonly Func<IEnumerable<string>, IEnumerable<string>> _xPathFilter;
        private readonly ITaskCollectionStepRepository _taskCollectionStepRepository;
        private readonly ICollectionStepsUpdater _collectionStepsUpdater;

        public CollectionStepsCreator(ICollectionStepsPolicyProvider collectionStepsPolicyProvider,
                                      ICollectionStepRepository collectionStepRepository,
                                      ITaskCollectionStepRepository taskCollectionStepRepository,
                                      ICollectionStepsUpdater collectionStepsUpdater,
                                      string jobName,
                                      Func<IEnumerable<string>, IEnumerable<string>> xPathFilter,
                                      ILogger logger)
        {
            _collectionStepsPolicyProvider = collectionStepsPolicyProvider ?? throw new ArgumentNullException(nameof(collectionStepsPolicyProvider));
            _collectionStepRepository = collectionStepRepository ?? throw new ArgumentNullException(nameof(collectionStepRepository));
            _xPathFilter = xPathFilter ?? throw new ArgumentNullException(nameof(xPathFilter));
            _taskCollectionStepRepository = taskCollectionStepRepository ?? throw new ArgumentNullException(nameof(taskCollectionStepRepository));
            _collectionStepsUpdater = collectionStepsUpdater ?? throw new ArgumentNullException(nameof(collectionStepsUpdater));
            _logger = logger?.ForContext("JobName", jobName) ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateStepsAsync(Guid userId,
                                           Guid policyId,
                                           Dictionary<string, NewCollectionStep> expectedSteps,
                                           TaskInfo[] tasks)
        {
            var logger = _logger.ForContext("UserId", userId)
                                .ForContext("PolicyId", policyId);

            var policy = await _collectionStepsPolicyProvider.GetAsync(policyId);
            var steps = Filter(policy.Steps);
            var stepsIds = await CreateStepsAsync(userId, steps, expectedSteps, logger);
            await AddStepsToTasksAsync(policy, tasks, stepsIds, logger);
        }

        private async Task<Dictionary<string, Guid>> CreateStepsAsync(Guid userId,
                                                                      IEnumerable<PolicyCollectionStep> policySteps,
                                                                      Dictionary<string, NewCollectionStep> expectedSteps,
                                                                      ILogger logger)
        {
            var stepsToSave = policySteps
                              .Join(expectedSteps,
                                    step => step.XPath,
                                    pair => pair.Key,
                                    (step, pair) => (ExtectedStep: pair.Value, StepVariant: step))
                              .ToDictionary(tuple => tuple.StepVariant, tuple => tuple.ExtectedStep);


            var notFoundSteps = policySteps.Select(step => step.XPath).Except(stepsToSave.Keys.Select(step => step.XPath)).ToArray();
            if (notFoundSteps.Length > 0)
                logger.Warning("No state resolved for {@XPathes}. Step creation skipped", notFoundSteps);
            

            var existingSteps = await _collectionStepRepository.FindAsync(userId, stepsToSave.Keys.Select(step => step.XPath).ToArray());
            return await _collectionStepsUpdater.SaveAsync(userId, stepsToSave, existingSteps);
        }

        private async Task AddStepsToTasksAsync(CollectionStepsPolicy policy,
                                                TaskInfo[] tasks,
                                                Dictionary<string, Guid> stepsIds,
                                                ILogger logger)
        {
            foreach (var variant in policy.Tasks)
            {
                var steps = stepsIds.Where(pair => pair.Key.In(variant.XPathes)).Select(pair => pair.Value).ToArray();
                if (steps.Length == 0)
                    continue;

                var task = tasks.FirstOrDefault(task => task.VariantId == variant.VariantId);
                if (task == null)
                {
                    logger.Warning("No task found with variant {VariantId} and {Type}", variant.VariantId, variant.Type);
                    continue;
                }

                try
                {
                    await _taskCollectionStepRepository.AddNewCollectionStepsAsync(task.Id, steps);
                }
                catch (SqlException e) when(e.IsUniqueKeyViolation())
                {
                    logger.Error(e, "Cannot link collection steps to tasks");
                    return;
                }
                
                var absentSteps = _xPathFilter(variant.XPathes).Except(stepsIds.Keys).ToArray();
                if (!absentSteps.IsNullOrEmpty())
                {
                    logger.Warning("Not all steps created for task: {TaskId}. "
                                 + "Variant: {TaskVariant}. Type: {TaskType}. "
                                 + "Absent steps: {AbsentSteps}.",
                                   task.Id,
                                   task.VariantId,
                                   task.Type,
                                   absentSteps);
                }
            }
        }

        private IEnumerable<PolicyCollectionStep> Filter(PolicyCollectionStep[] policySteps)
        {
            var allowedSteps = _xPathFilter(policySteps.Select(step => step.XPath));
            return policySteps.Where(step => step.XPath.In(allowedSteps));
        }

        public class TaskInfo
        {
            public Guid Id { get; set; }

            public Guid VariantId { get; set; }

            public TaskType Type { get; set; }
        }
    }
}