using System;
using System.Linq;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;
using CollectionStep = WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models.CollectionStep;

namespace WX.B2C.User.Verification.Unit.Tests.Jobs.Builders
{
    internal interface ICollectionStepBuilder : IUserConsistencyBuilder
    {
        ICollectionStepBuilder With(Guid id);

        ICollectionStepBuilder With(string xPath);

        ICollectionStepBuilder Required(bool isRequired = true);

        ICollectionStepBuilder ReviewRequired(bool isReviewRequired = true);

        ICollectionStepBuilder With(CollectionStepState state);

        ICollectionStepBuilder With(CollectionStepReviewResult? reviewResult);

        ICollectionStepBuilder With(DateTime createdAt);

        ICollectionStepBuilder AddRelatedTasks(params Guid[] taskIds);
    }

    internal class CollectionStepBuilder : UserConsistencyBuilder, ICollectionStepBuilder
    {
        private readonly CollectionStep _step;

        public CollectionStepBuilder(UserConsistency user) 
            : base(user)
        {
            _step = new CollectionStep();
            _user.CollectionSteps ??= Array.Empty<CollectionStep>();
            _user.CollectionSteps = _user.CollectionSteps.Concat(new[] { _step }).ToArray();
        }

        public ICollectionStepBuilder With(Guid id) =>
            Update(step => step.Id = id);

        public ICollectionStepBuilder Required(bool isRequired = true) =>
            Update(step => step.IsRequired = isRequired);

        public ICollectionStepBuilder ReviewRequired(bool isReviewRequired = true) =>
            Update(step => step.IsReviewRequired = isReviewRequired);

        public ICollectionStepBuilder With(string xPath) =>
            Update(step => step.XPath = xPath);

        public ICollectionStepBuilder With(CollectionStepState state) =>
            Update(step => step.State = state);

        public ICollectionStepBuilder With(CollectionStepReviewResult? reviewResult) =>
            Update(step => step.Result = reviewResult);

        public ICollectionStepBuilder With(DateTime createdAt) =>
            Update(step => step.CreatedAt = createdAt);

        public ICollectionStepBuilder AddRelatedTasks(params Guid[] taskIds)
        {
            var relatedTasks = (_step.RelatedTasks ?? Array.Empty<Guid>()).Concat(taskIds);
            return Update(step => step.RelatedTasks = relatedTasks.ToArray());
        }
        private ICollectionStepBuilder Update(Action<CollectionStep> update)
        {
            update(_step);
            return this;
        }
    }
}
