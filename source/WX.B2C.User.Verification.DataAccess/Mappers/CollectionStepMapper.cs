using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface ICollectionStepMapper
    {
        CollectionStepDto MapToDto(Entities.CollectionStep entity);

        CollectionStep Map(Entities.CollectionStep entity);

        Entities.CollectionStep Map(CollectionStep collectionStep);

        void Update(CollectionStep collectionStep, Entities.CollectionStep entity);
    }

    internal class CollectionStepMapper : ICollectionStepMapper
    {
        public CollectionStepDto MapToDto(Entities.CollectionStep entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new CollectionStepDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                XPath = entity.XPath,
                State = entity.State,
                IsRequired = entity.IsRequired,
                IsReviewNeeded = entity.IsReviewNeeded,
                ReviewResult = entity.ReviewResult,
                RequestedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }

        public CollectionStep Map(Entities.CollectionStep entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new CollectionStep(
                entity.Id, 
                entity.UserId, 
                entity.XPath, 
                entity.State, 
                entity.IsRequired, 
                entity.IsReviewNeeded,
                entity.ReviewResult);
        }

        public Entities.CollectionStep Map(CollectionStep collectionStep)
        {
            if (collectionStep == null)
                throw new ArgumentNullException(nameof(collectionStep));

            var entity = new Entities.CollectionStep { Id = collectionStep.Id };
            Update(collectionStep, entity);
            return entity;
        }

        public void Update(CollectionStep collectionStep, Entities.CollectionStep entity)
        {
            if (collectionStep == null)
                throw new ArgumentNullException(nameof(collectionStep));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.UserId = collectionStep.UserId;
            entity.XPath = collectionStep.XPath;
            entity.IsRequired = collectionStep.IsRequired;
            entity.IsReviewNeeded = collectionStep.IsReviewNeeded;
            entity.State = collectionStep.State;
            entity.ReviewResult = collectionStep.ReviewResult;
        }
    }
}