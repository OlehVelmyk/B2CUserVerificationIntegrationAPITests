using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using NSubstitute;
using NUnit.Framework;
using Quartz;
using Quartz.Spi;
using WX.B2C.User.Verification.Worker.Jobs.Quartz;

namespace WX.B2C.User.Verification.Unit.Tests.Jobs
{
    internal class AlwaysFailsJob : IJob
    {
        public int Counter { get; private set; }

        public Task Execute(IJobExecutionContext context)
        {
            Counter++;
            throw new NotImplementedException();
        }
    }
    internal class ResetLockWhenJobRetriesExceeded : IJobRetryStrategy
    {
        private readonly IJobRetryStrategy _inner;
        private readonly ManualResetEvent _reset;

        public ResetLockWhenJobRetriesExceeded(
            IJobRetryStrategy inner,
            ManualResetEvent reset)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _reset = reset ?? throw new ArgumentNullException(nameof(reset));
        }
        public bool ShouldRetry(IJobExecutionContext context)
        {
            var shouldRetry = _inner.ShouldRetry(context);
            if (!shouldRetry)
                _reset.Set();

            return shouldRetry;
        }

        public ITrigger GetTrigger(IJobExecutionContext context) => _inner.GetTrigger(context);
    }

    public class RetryJobStrategyTests
    {
        [Test]
        public async Task ShouldTryThreeTimes_ThenGiveUp()
        {
            // Arrange
            var settings = new JobRetrySettings(2, 250.Milliseconds());
            var sut = new ExponentialBackoffRetryStrategy(settings);
            var reset = new ManualResetEvent(false);
            var retryStrategy = new ResetLockWhenJobRetriesExceeded(sut, reset);

            var propertiesProvider = Substitute.For<IQuartzPropertiesProvider>();
            propertiesProvider.GetProperties()
                              .Returns(new NameValueCollection());

            var schedulerInitializer = Substitute.For<IJobSchedulerInitializer>();
            var jobFactory = Substitute.For<IJobFactory>();
            var factory = new JobSchedulerFactory(propertiesProvider, schedulerInitializer, jobFactory);

            var alwaysFails = new AlwaysFailsJob();
            var decoratedJob = new EnsureJobExecutionExceptionDecorator(alwaysFails);
            jobFactory.NewJob(Arg.Any<TriggerFiredBundle>(), Arg.Any<IScheduler>())
                      .Returns(decoratedJob);

            var job = JobBuilder
                      .Create<AlwaysFailsJob>()
                      .WithIdentity("always-fails-job")
                      .Build();

            var trigger = TriggerBuilder
                          .Create()
                          .StartNow()
                          .WithIdentity("always-fails-trigger")
                          .WithSimpleSchedule(
                              x =>
                              {
                                  x.WithIntervalInSeconds(1);
                                  x.WithRepeatCount(0);
                              })
                          .Build();

            // Act
            var scheduler = await factory.Create(retryStrategy);
            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
            await scheduler.ResumeAll();

            // Assert
            reset.WaitOne(15.Seconds());
            alwaysFails.Counter.Should().Be(settings.MaxRetries + 1);
        }
    }
}
