using System;
using System.Threading.Tasks;
using Quartz;

namespace WX.B2C.User.Verification.Worker.Jobs.Quartz
{
    internal sealed class EnsureJobExecutionExceptionDecorator : IJob
    {
        private readonly IJob _inner;

        public EnsureJobExecutionExceptionDecorator(IJob inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                return _inner.Execute(context);
            }
            catch (JobExecutionException)
            {
                throw;
            }
            catch (Exception cause)
            {
                throw new JobExecutionException(cause);
            }
        }
    }
}
