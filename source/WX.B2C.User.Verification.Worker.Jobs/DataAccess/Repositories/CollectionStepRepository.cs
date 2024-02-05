using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories
{
    internal interface ICollectionStepRepository
    {
        Task<CollectionStepEntity[]> FindAsync(Guid userId, params string[] xPath);

        Task CreateAsync(CollectionStepEntity[] entities);

        Task UpdateAsync(CollectionStepEntity[] entities);

        Task UpdateAsync(Guid[] collectionStepsIds, CollectionStepUpdatePatch patch);

        Task DeleteAsync(Guid[] collectionStepsIds);
    }

    internal class CollectionStepRepository : ICollectionStepRepository
    {
        private const string CollectionStepsTableName = "CollectionSteps";
        private static readonly string[] ColumnsToInsert =
        {
            nameof(CollectionStepEntity.Id),
            nameof(CollectionStepEntity.UserId),
            nameof(CollectionStepEntity.XPath),
            nameof(CollectionStepEntity.IsRequired),
            nameof(CollectionStepEntity.IsReviewNeeded),
            nameof(CollectionStepEntity.State),
            nameof(CollectionStepEntity.ReviewResult),
            nameof(CollectionStepEntity.CreatedAt),
            nameof(CollectionStepEntity.UpdatedAt)
        };

        private static readonly string[] ColumnsToUpdate =
        {
            nameof(CollectionStepEntity.IsRequired),
            nameof(CollectionStepEntity.IsReviewNeeded),
            nameof(CollectionStepEntity.State),
            nameof(CollectionStepEntity.ReviewResult),
            nameof(CollectionStepEntity.UpdatedAt)
        };

        private readonly IQueryFactory _queryFactory;

        public CollectionStepRepository(IQueryFactory dbQueryFactory)
        {
            _queryFactory = dbQueryFactory ?? throw new ArgumentNullException(nameof(dbQueryFactory));
        }

        public async Task<CollectionStepEntity[]> FindAsync(Guid userId, params string[] xPath)
        {
            var query = new Query(CollectionStepsTableName).Select("*")
                                                           .Where(nameof(CollectionStepEntity.UserId), userId)
                                                           .WhereIn(nameof(CollectionStepEntity.XPath), xPath);

            using var queryFactory = _queryFactory.Create();
            var allSteps = await queryFactory.GetAsync<CollectionStepEntity>(query);

            var actualSteps = allSteps.GroupBy(entity => entity.XPath)
                                      .Select(steps => steps.OrderBy(dto => dto.State)
                                                            .ThenByDescending(dto => dto.CreatedAt)
                                                            .First())
                                      .ToArray();
            return actualSteps;
        }

        public async Task CreateAsync(CollectionStepEntity[] entities)
        {
            if (entities.Length == 0)
                return;

            var values = ToInsertValues(entities);
            var query = new Query(CollectionStepsTableName).AsInsert(ColumnsToInsert, values);
            using var queryFactory = _queryFactory.Create();

            await queryFactory.ExecuteAsync(query);
        }

        public async Task UpdateAsync(CollectionStepEntity[] entities)
        {
            if (entities.Length == 0)
                return;

            using var queryFactory = _queryFactory.Create();
            await entities.Foreach(entity =>
            {
                var query = new Query(CollectionStepsTableName)
                            .AsUpdate(ColumnsToUpdate, ToUpdateValues(entity))
                            .Where(nameof(CollectionStepEntity.Id), entity.Id);
                return queryFactory.ExecuteAsync(query);
            });
        }

        public async Task UpdateAsync(Guid[] collectionStepsIds, CollectionStepUpdatePatch patch)
        {
            var values = new Dictionary<string, object>();
            patch.IsRequired.MatchSome(isRequired => values.Add(nameof(patch.IsRequired), isRequired));
            patch.IsReviewNeeded.MatchSome(isReviewNeeded => values.Add(nameof(patch.IsReviewNeeded), isReviewNeeded.ToString()));
            patch.State.MatchSome(state => values.Add(nameof(patch.State), state.ToString()));
            patch.ReviewResult.MatchSome(result => values.Add(nameof(patch.ReviewResult), result?.ToString()));
            
            using var queryFactory = _queryFactory.Create();
            var query = new Query(CollectionStepsTableName)
                        .WhereIn(nameof(CollectionStepEntity.Id), collectionStepsIds)
                        .AsUpdate(values);
                        
            await queryFactory.ExecuteAsync(query);
        }

        public async Task DeleteAsync(Guid[] collectionStepsIds)
        {
            using var queryFactory = _queryFactory.Create();
            var query = new Query(CollectionStepsTableName)
                        .WhereIn(nameof(CollectionStepEntity.Id), collectionStepsIds)
                        .AsDelete();
                        
            await queryFactory.ExecuteAsync(query);
        }

        private IEnumerable<IEnumerable<object>> ToInsertValues(CollectionStepEntity[] entities) =>
            entities.Select(ToInsertValues);

        private IEnumerable<object> ToInsertValues(CollectionStepEntity entity) =>
            new object[]
            {
                entity.Id,
                entity.UserId,
                entity.XPath,
                entity.IsRequired,
                entity.IsReviewNeeded,
                entity.State.ToString(),
                entity.ReviewResult?.ToString(),
                entity.CreatedAt,
                entity.UpdatedAt
            };

        private IEnumerable<object> ToUpdateValues(CollectionStepEntity entity) =>
            new object[]
            {
                entity.IsRequired,
                entity.IsReviewNeeded,
                entity.State.ToString(),
                entity.ReviewResult?.ToString(),
                entity.UpdatedAt
            };
    }

    internal class CollectionStepEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string XPath { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }

        public CollectionStepState State { get; set; }

        public CollectionStepReviewResult? ReviewResult { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}