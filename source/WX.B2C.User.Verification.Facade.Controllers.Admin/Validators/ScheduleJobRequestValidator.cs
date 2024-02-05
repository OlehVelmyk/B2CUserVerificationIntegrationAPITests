using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class ScheduleJobRequestValidator : BaseRequestValidator<ScheduleJobRequest>
    {
        public ScheduleJobRequestValidator()
        {
            RuleFor(request => request.JobName).NotEmpty();
            RuleFor(request => request.JobParameters).NotNull();

            RuleFor(request => request.Schedule.Type)
                .IsInEnum()
                .When(request => request.Schedule != null);

            RuleFor(request => request.Schedule as CronJobSchedule)
                .SetValidator(new CronJobScheduleValidator())
                .When(request => request.Schedule is { Type: JobScheduleType.Cron });

            RuleFor(request => request.Schedule as IntervalJobSchedule)
                .SetValidator(new IntervalJobScheduleValidator())
                .When(request => request.Schedule is { Type: JobScheduleType.Interval });
        }
    }

    public class IntervalJobScheduleValidator : AbstractValidator<IntervalJobSchedule>
    {
        public IntervalJobScheduleValidator()
        {
            RuleFor(schedule => schedule.Type)
                .IsInEnum()
                .Equal(JobScheduleType.Interval);

            RuleFor(schedule => schedule.Unit).IsInEnum();
            RuleFor(schedule => schedule.Value).GreaterThan(0);
        }
    }

    public class CronJobScheduleValidator : AbstractValidator<CronJobSchedule>
    {
        public CronJobScheduleValidator()
        {
            RuleFor(schedule => schedule.Type)
                .IsInEnum()
                .Equal(JobScheduleType.Cron);

            RuleFor(schedule => schedule.Cron).NotEmpty();
        }
    }
}
