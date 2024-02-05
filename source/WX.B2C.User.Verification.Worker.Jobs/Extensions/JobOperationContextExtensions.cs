using System;
using Newtonsoft.Json;
using Quartz;
using WX.B2C.User.Verification.Worker.Jobs.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Extensions
{
    public class OperationContext
    {
        public Guid CorrelationId { get; set; }

        public Guid OperationId { get; set; }

        public string OperationName { get; set; }
    }

    internal static class JobOperationContextExtensions
    {
        public static OperationContext GetOperationContext(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var serializedData = context.MergedJobDataMap.GetString(Constants.JobOperationContext);

            if (string.IsNullOrEmpty(serializedData))
            {
                return new OperationContext
                {
                    CorrelationId = Guid.NewGuid(),
                    OperationId = Guid.NewGuid(),
                    OperationName = $"Job: {context.JobDetail.JobType.Name}"
                };
            }

            return JsonConvert.DeserializeObject<OperationContext>(serializedData);
        }
    }
}
