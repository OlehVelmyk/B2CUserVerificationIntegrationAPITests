using System;
using System.Threading.Tasks;
using Quartz;
using Serilog;

namespace WX.B2C.User.Verification.Worker.Jobs.Quartz
{
    internal sealed class JobExecutionLoggingDecorator : IJob
    {
        private readonly IJob _inner;
        private readonly ILogger _logger;

        public JobExecutionLoggingDecorator(IJob inner, ILogger logger)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var logger = _logger.ForContext(nameof(context.JobDetail.JobType), context.JobDetail.JobType)
                                .ForContext(nameof(context.JobDetail.Key), context.JobDetail.Key)
                                .ForContext(nameof(context.FireInstanceId), context.FireInstanceId);
            try
            {
                logger.Information("Job execution started");
                await _inner.Execute(context);
                logger.Information("Job execution finished");
            }
            catch (Exception e)
            {
                logger.Error(e, "Job execution failed");
                throw;
            }
        }
    }
}