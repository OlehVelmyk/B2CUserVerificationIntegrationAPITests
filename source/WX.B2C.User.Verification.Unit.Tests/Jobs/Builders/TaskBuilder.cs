using System;
using System.Linq;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;

namespace WX.B2C.User.Verification.Unit.Tests.Jobs.Builders
{
    internal interface ITaskBuilder : IUserConsistencyBuilder
    {
        ITaskBuilder With(Guid id);

        ITaskBuilder With(TaskState state);

        ITaskBuilder With(TaskResult? result);

        ITaskBuilder With(TaskType type);

        ITaskBuilder WithApplicationId(Guid applicationId);
    }

    internal class TaskBuilder : UserConsistencyBuilder, ITaskBuilder
    {
        private readonly Task _task;

        public TaskBuilder(UserConsistency user)
            : base(user)
        {
            _task = new Task();
            _user.Tasks ??= Array.Empty<Task>();
            _user.Tasks = _user.Tasks.Concat(new[] { _task }).ToArray();
        }

        public ITaskBuilder With(Guid id) =>
            Update(task => task.Id = id);

        public ITaskBuilder With(TaskState state) =>
            Update(task => task.State = state);

        public ITaskBuilder With(TaskResult? result) =>
            Update(task => task.Result = result);

        public ITaskBuilder With(TaskType type) =>
            Update(task => task.Type = type);

        public ITaskBuilder WithApplicationId(Guid applicationId) =>
            Update(task => task.ApplicationId = applicationId);

        private ITaskBuilder Update(Action<Task> update)
        {
            update(_task);
            return this;
        }
    }
}
