using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Services
{
    internal class JobService : IJobService
    {
        private readonly IScheduler _jobScheduler;
        private readonly IJobBuilderProvider _jobBuilderProvider;
        private readonly ITriggerKeyProvider _triggerKeyProvider;
        private readonly IJobKeyProvider _jobKeyProvider;
        private readonly IOperationContextProvider _operationContextProvider;
        private readonly ILogger _logger;

        public JobService(IScheduler jobScheduler,
                          IJobBuilderProvider jobBuilderProvider,
                          ITriggerKeyProvider triggerKeyProvider,
                          IOperationContextProvider operationContextProvider,
                          IJobKeyProvider jobKeyProvider,
                          ILogger logger)
        {
            _jobScheduler = jobScheduler ?? throw new ArgumentNullException(nameof(jobScheduler));
            _jobBuilderProvider = jobBuilderProvider ?? throw new ArgumentNullException(nameof(jobBuilderProvider));
            _triggerKeyProvider = triggerKeyProvider ?? throw new ArgumentNullException(nameof(triggerKeyProvider));
            _jobKeyProvider = jobKeyProvider ?? throw new ArgumentNullException(nameof(jobKeyProvider));
            _operationContextProvider = operationContextProvider ?? throw new ArgumentNullException(nameof(operationContextProvider));
            _logger = logger?.ForContext<JobService>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ScheduleAsync(JobRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var jobSettingsMap = request.Parameters?.ToDictionary(x => x.Name, x => x.Value);
                var jobBuilder = _jobBuilderProvider.Get(request.JobName, jobSettingsMap);

                var job = jobBuilder.Build();

                var triggerKey = _triggerKeyProvider.Get(request.JobName, jobSettingsMap);
                var triggerBuilder = TriggerBuilder
                                     .Create()
                                     .WithIdentity(triggerKey)
                                     .ForJob(job);

                if (request.Parameters != null)
                {
                    var jobSettings = JsonConvert.SerializeObject(jobSettingsMap);
                    triggerBuilder = triggerBuilder.UsingJobData(Constants.JobSettings, jobSettings);
                }

                var operationContext = _operationContextProvider.GetContextOrDefault();
                var operationContextSerialized = JsonConvert.SerializeObject(operationContext);
                triggerBuilder = triggerBuilder.UsingJobData(Constants.JobOperationContext, operationContextSerialized);

                triggerBuilder = request.StartAt.HasValue
                    ? triggerBuilder.StartAt(request.StartAt.Value)
                    : triggerBuilder.StartNow();

                triggerBuilder = request.Schedule switch
                {
                    CronSchedule cronSchedule         => triggerBuilder.WithCronSchedule(cronSchedule.Cron),
                    IntervalSchedule intervalSchedule => triggerBuilder.WithIntervalSchedule(intervalSchedule),
                    null                              => triggerBuilder.WithSimpleSchedule(),
                    _                                 => throw new ArgumentOutOfRangeException()
                };

                var trigger = triggerBuilder.Build();

                var isJobExists = await _jobScheduler.CheckExists(job.Key, cancellationToken);
                var isTriggerExists = await _jobScheduler.CheckExists(triggerKey, cancellationToken);
                DateTimeOffset? offset = null;

                if (isJobExists && isTriggerExists)
                    offset = await _jobScheduler.RescheduleJob(triggerKey, trigger, cancellationToken);
                else if (isJobExists)
                    offset = await _jobScheduler.ScheduleJob(trigger, cancellationToken);
                else
                    offset = await _jobScheduler.ScheduleJob(job, trigger, cancellationToken);

                if (!offset.HasValue)
                {
                    _logger.Warning("Trigger {TriggerKey} for {JobKey} is not scheduled. Please retry operation", triggerKey, job.Key);
                }
                else
                {
                    var nextTime = trigger.GetNextFireTimeUtc();
                    _logger.Information("Trigger {TriggerKey} for {JobKey} will be raised {NextRaiseTime}. Offset {Offset}", triggerKey, job.Key, nextTime, offset.Value);
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Schedule job request failed.");
            }
        }

        public async Task<bool> UnscheduleAsync(JobParametersDto parameters, CancellationToken cancellationToken = default)
        {
            var triggerKey = ResolveTriggerKey(parameters);
            
            try
            {
                var isTriggerExists = await _jobScheduler.CheckExists(triggerKey, cancellationToken);
                if (isTriggerExists && await _jobScheduler.UnscheduleJob(triggerKey, cancellationToken))
                {
                    _logger.Information("Unscheduled job ({JobName}) trigger ({@TriggerKey})", parameters.JobName, triggerKey);
                    var jobs = await _jobScheduler.GetCurrentlyExecutingJobs(cancellationToken);
                    var executedInstances = jobs.Where(context => context.Trigger.Key.Equals(triggerKey))
                                                .Select(context => context.FireInstanceId);
                    await executedInstances.Foreach(fireInstanceId => InterruptAsync(fireInstanceId, CancellationToken.None));

                    return true;
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Cannot unschedule job ({JobName}) trigger ({@TriggerKey})", parameters.JobName, triggerKey);
            }

            return false;
        }
        
        /// <summary>
        /// Unschedule all triggers connected to job.
        /// Note this method is allowed only if job provider JobKeyExtractors
        /// </summary>
        public async Task UnscheduleJobAsync(JobParametersDto parameters, CancellationToken cancellationToken = default)
        {
            var jobSettingsMap = parameters.Parameters?.ToDictionary(x => x.Name, x => x.Value);
            var jobKey = _jobKeyProvider.Get(parameters.JobName, jobSettingsMap);
            
            var isJobExists = await _jobScheduler.CheckExists(jobKey, cancellationToken);
            if (!isJobExists)
                return;
                
            var triggers = await _jobScheduler.GetTriggersOfJob(jobKey, cancellationToken);
            var triggerKeys = triggers.Select(trigger => trigger.Key).ToArray();    
            await _jobScheduler.UnscheduleJobs(triggerKeys, cancellationToken);
        }

        public async Task InterruptAsync(string fireInstanceId, CancellationToken cancellationToken)
        {
            if (await _jobScheduler.Interrupt(fireInstanceId, cancellationToken))
                _logger.Information("Interrupted trigger instance: {FireTriggerId}", fireInstanceId);
            else 
                _logger.Information("Cannot interrupt trigger instance: {FireTriggerId}", fireInstanceId);
        }

        public async Task<bool> ExistsTriggerAsync(JobParametersDto jobParameters)
        {
            var triggerKey = ResolveTriggerKey(jobParameters);
            var existsTrigger = await _jobScheduler.CheckExists(triggerKey);
            if (!existsTrigger)
                return false;

            var trigger = await _jobScheduler.GetTrigger(triggerKey);
            if (trigger == null)
                return false;

            var mayFireAgain = trigger.GetMayFireAgain();
            var nextTimeFire = trigger.GetNextFireTimeUtc();
            var endTimeUtc = trigger.EndTimeUtc;
            var currentTime = DateTime.UtcNow;
            _logger.Information(
            "Existing trigger {@TriggerKey} info: May fire again: {MayFireAgain}. Next time: {GetNextFireTimeUtc}. EndTime: {EndTimeUtc} Current time: {CurrentTime} ",
            triggerKey,
            mayFireAgain,
            nextTimeFire,
            endTimeUtc,
            currentTime);

            return mayFireAgain || ( endTimeUtc.HasValue && endTimeUtc.Value > currentTime);
        }

        public async Task<JobTriggerDto[]> GetJobTriggersAsync(JobParametersDto parameters, CancellationToken cancellationToken)
        {
            var jobSettingsMap = parameters.Parameters?.ToDictionary(x => x.Name, x => x.Value);
            var jobKey = _jobKeyProvider.Get(parameters.JobName, jobSettingsMap);
            
            var isJobExists = await _jobScheduler.CheckExists(jobKey, cancellationToken);
            if (!isJobExists)
                return Array.Empty<JobTriggerDto>();
                
            var triggers = await _jobScheduler.GetTriggersOfJob(jobKey, cancellationToken);
            var jobTrigger = triggers.Where(trigger => trigger.GetMayFireAgain())
                                     .Select(trigger => new JobTriggerDto
                                     {
                                         TriggerId = trigger.Key.Name,
                                         JobName = jobKey.Name,
                                         JobKey = jobKey.Group,
                                         NextFireTime = trigger.GetNextFireTimeUtc().Value.UtcDateTime
                                     })
                                     .ToArray();
            return jobTrigger;
        }

        private TriggerKey ResolveTriggerKey(JobParametersDto jobParameters)
        {
            try
            {
                var jobSettingsMap = jobParameters.Parameters?.ToDictionary(x => x.Name, x => x.Value);
                var triggerKey = _triggerKeyProvider.Get(jobParameters.JobName, jobSettingsMap);
                return triggerKey;
            }
            catch (Exception e)
            {
                _logger.Error(e,
                              "Cannot resolve trigger key for job {JobName}. Parameters {@Parameters}",
                              jobParameters.JobName,
                              jobParameters.Parameters);
                throw;
            }
        }
    }
}