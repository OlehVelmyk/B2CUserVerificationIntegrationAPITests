using System;
using System.Collections.Generic;
using System.Linq;
using SqlKata;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks
{
    /// <summary>
    /// Provide static tasks (tasks which should be added to application in the beginning of verification)
    /// </summary>
    internal class StaticTasksProvider : IStaticTasksProvider
    {
        private readonly IQueryFactory _queryFactory;
        private readonly Lazy<Dictionary<Guid, TaskVariant[]>> _lazyPolicyStaticTasks;

        public StaticTasksProvider(IQueryFactory queryFactory)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
            _lazyPolicyStaticTasks = new Lazy<Dictionary<Guid, TaskVariant[]>>(() => ReadTaskVariants());
        }

        public TaskVariant Find(Guid policyId, TaskType taskType)
        {
            var taskVariants = _lazyPolicyStaticTasks.Value[policyId];
            return taskVariants.FirstOrDefault(tv => tv.Type == taskType);
        }

        public TaskVariant[] Get(Guid policyId) =>
            _lazyPolicyStaticTasks.Value[policyId];

        private Dictionary<Guid, TaskVariant[]> ReadTaskVariants()
        {
            var factory = _queryFactory.Create();
            var query = new Query("Policy.PolicyTasks as PT")
                               .Select("PT.PolicyId", "T.Id as VariantId", "T.Type")
                               .Join("Policy.Tasks as T", "PT.TaskVariantId", "T.Id");
            
            var data = factory.Get<DbData>(query);
            return data.GroupBy(db => db.PolicyId, db => new TaskVariant { Id = db.VariantId, Type = db.Type })
                       .ToDictionary(g => g.Key, g => g.ToArray());
        }

        private class DbData
        {
            public Guid PolicyId { get; set; }

            public Guid VariantId { get; set; }

            public TaskType Type { get; set; }
        }
    }
}
