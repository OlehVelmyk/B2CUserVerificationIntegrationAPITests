using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using ICollectionStepRepository = WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories.ICollectionStepRepository;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal class CreateStepStateJob : BatchJob<UserTasks, CreateStepStateJobSettings>
    {
        private readonly ICollectionStepRepository _stepRepository;
        private readonly ITaskCollectionStepRepository _taskRepository;
        private readonly CollectionStepVariantValidator _validator;

        public CreateStepStateJob(ICreateStepStateJobProvider jobDataProvider,
                                  ICollectionStepRepository stepRepository,
                                  ITaskCollectionStepRepository taskRepository,
                                  IXPathParser xPathParser,
                                  ILogger logger) : base(jobDataProvider, logger)
        {
            _stepRepository = stepRepository ?? throw new ArgumentNullException(nameof(stepRepository));
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _validator = new CollectionStepVariantValidator(xPathParser);
        }

        internal static string Name => "create-step";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<CreateStepStateJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Create collection steps")
                 .StoreDurably();

        protected override async Task Execute(Batch<UserTasks> batch,
                                              CreateStepStateJobSettings settings,
                                              CancellationToken cancellationToken)
        {
            // TODO implement later validation mechanism
            if (settings.TaskTypes.IsNullOrEmpty())
                throw new ArgumentException("Must not be null or empty",nameof(settings.TaskTypes));

            _validator.ValidateAndThrow(settings.Variant);


            var (valid, skipped) = ValidateUserTasks(batch.Items, settings.TaskTypes);
            if (skipped.Count > 0)
                Logger.Warning("Not all tasks exists for creating steps. Skipped users: {@SkippedUsers}", skipped);

            var collectionSteps = valid.Select(user => Map(user.UserId, settings.Variant)).ToArray();
            await _stepRepository.CreateAsync(collectionSteps);

            var taskSteps = collectionSteps.Join(valid,
                                                 entity => entity.UserId,
                                                 tasks => tasks.UserId,
                                                 (step, tasks) => tasks.Tasks.Select(task => (task.Id, step.Id)))
                                           .Flatten();
                                           
            await _taskRepository.LinkAsync(taskSteps);
        }

        private (ICollection<UserTasks> validUsers, ICollection<UserTasks> skippedUsers) ValidateUserTasks(UserTasks[] usersTasks, TaskType[] taskTypes)
        {
            var validUsers = new List<UserTasks>();
            var skippedUsers = new List<UserTasks>();
            foreach (var userTasks in usersTasks)
            {
                var isMissed = taskTypes.Except(userTasks.Tasks.Select(task => task.Type)).Any();
                if (isMissed)
                    skippedUsers.Add(userTasks);
                else validUsers.Add(userTasks);
            }

            return (validUsers, skippedUsers);
        }

        private CollectionStepEntity Map(Guid userId, CollectionStepVariant variant) =>
            new()
            {
                Id = Guid.NewGuid(),
                XPath = variant.XPath,
                State = variant.State,
                CreatedAt = DateTime.UtcNow,
                IsRequired = variant.IsRequired,
                IsReviewNeeded = variant.IsReviewRequired,
                ReviewResult = variant.Result,
                UserId = userId,
            };
    }

    internal class UserTasks : IJobData
    {
        public Guid UserId { get; set; }

        public Task[] Tasks { get; set; }

        internal class Task
        {
            public Guid Id { get; set; }

            public TaskType Type { get; set; }
        }
    }

    internal class CollectionStepVariantValidator : AbstractValidator<CollectionStepVariant>
    {
        public CollectionStepVariantValidator(IXPathParser xPathParser)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(step => step.XPath).NotNull();
            RuleFor(step => step.XPath).Must(xPathParser.IsValid);

            RuleFor(step => step.State).IsInEnum();
            RuleFor(step => step.State)
                .NotEqual(CollectionStepState.InReview)
                .When(step => !step.IsReviewRequired);

            RuleFor(step => step.Result)
                .NotNull()
                .When(step => step.IsReviewRequired && step.State == CollectionStepState.Completed);

            RuleFor(step => step.Result)
                .Null()
                .When(step => !step.IsReviewRequired || step.State != CollectionStepState.Completed);
            RuleFor(step => step.Result).IsInEnum().When(step => step.Result.HasValue);
        }
    }
}