using System;
using System.Linq;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;
using Check = WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models.Check;

namespace WX.B2C.User.Verification.Unit.Tests.Jobs.Builders
{
    internal interface ICheckBuilder : IUserConsistencyBuilder
    {
        ICheckBuilder With(CheckState state);

        ICheckBuilder AddRelatedTasks(params Guid[] taskIds);
    }

    internal class CheckBuilder : UserConsistencyBuilder, ICheckBuilder
    {
        private readonly Check _check;

        public CheckBuilder(UserConsistency user) 
            : base(user)
        {
            _check = new Check();
            _user.Checks ??= Array.Empty<Check>();
            _user.Checks = _user.Checks.Concat(new[] { _check }).ToArray();
        }

        public ICheckBuilder With(CheckState state) =>
            Update(check => check.State = state);

        public ICheckBuilder AddRelatedTasks(params Guid[] taskIds)
        {
            var relatedTasks = (_check.RelatedTasks ?? Array.Empty<Guid>()).Concat(taskIds);
            return Update(check => check.RelatedTasks = relatedTasks.ToArray());
        }

        private ICheckBuilder Update(Action<Check> update)
        {
            update(_check);
            return this;
        }
    }
}
