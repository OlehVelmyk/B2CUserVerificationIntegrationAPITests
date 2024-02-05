using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories
{
    internal interface ICheckRepository
    {
        Task<(Guid CheckId, Guid VariantId)[]> InstructAsync(Guid userId, CheckVariantInfo[] checksVariants);

        Task CancelChecksAsync(IEnumerable<Guid> checks, CancelRunningInstruction instructionToCancel);
    }

    internal class CheckRepository : ICheckRepository
    {
        private const string TableName = "Checks";

        private readonly IQueryFactory _queryFactory;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new();

        public CheckRepository(IQueryFactory dbQueryFactory, ILogger logger)
        {
            _queryFactory = dbQueryFactory ?? throw new ArgumentNullException(nameof(dbQueryFactory));
            _logger = logger?.ForContext<CheckRepository>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(Guid CheckId, Guid VariantId)[]> InstructAsync(Guid userId, CheckVariantInfo[] checksVariants)
        {
            using var factory = _queryFactory.Create();
            var columns = new[]
            {
                "Id",
                "UserId",
                "Type",
                "VariantId",
                "Provider",
                "State",
                "CreatedAt",
            };
            var checks = checksVariants.Select(info => new object[]
                                       {
                                           Guid.NewGuid(),
                                           userId,
                                           info.Type.ToString(),
                                           info.Id,
                                           info.Provider.ToString(),
                                           CheckState.Pending.ToString(),
                                           DateTime.UtcNow
                                       })
                                       .ToArray();

            var insert = factory.Query(TableName)
                                .InsertAsync(columns, checks);

            await insert;

            return checks.Select(list => ((Guid) list[0], (Guid) list[3])).ToArray();
        }

        public async Task CancelChecksAsync(IEnumerable<Guid> checks, CancelRunningInstruction instructionToCancel)
        {
            if (checks == null)
                throw new ArgumentNullException(nameof(checks));
            if (instructionToCancel == null)
                throw new ArgumentNullException(nameof(instructionToCancel));

            var checkIds = checks as Guid[] ?? checks.ToArray();
            if (checkIds.Length == 0)
                return;

            using var factory = _queryFactory.Create();

            var errors = JsonConvert.SerializeObject(new[] { instructionToCancel.CheckError }, _jsonSerializerSettings);
            var completedAt = DateTime.UtcNow;
            var values = new Dictionary<string, object>
            {
                ["State"] = CheckState.Error.ToString(),
                ["Errors"] = errors,
                ["CompletedAt"] = completedAt
            };

            using var queryFactory = _queryFactory.Create();
            var query = new Query(TableName)
                        .WhereIn("Id", checkIds)
                        .Where("State", "Running")
                        .AsUpdate(values);

            var affected = await queryFactory.ExecuteAsync(query);
            if (affected == checkIds.Length)
                return;

            _logger.Warning("Some of provided checks was not cancelled as not in running state. {@ProvidedChecks}. " + 
                            "Completed at for filtering {CompletedAt}",
                            checkIds,
                            completedAt);
        }
    }
}