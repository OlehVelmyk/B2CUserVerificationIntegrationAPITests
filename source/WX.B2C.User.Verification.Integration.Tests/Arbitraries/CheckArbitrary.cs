using System;
using System.Collections.Generic;
using FsCheck;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class CheckArbitrary : Arbitrary<CheckSpecimen>
    {
        public static Arbitrary<CheckSpecimen> Create() => new CheckArbitrary();

        public override Gen<CheckSpecimen> Generator =>
            from id in Arb.Generate<Guid>()
            from userId in Arb.Generate<Guid>()
            from variant in CheckVariants
            from type in Arb.Generate<CheckType>()
            from state in Arb.Generate<CheckState>()
            from executionContext in ExecutionContext(state)
            from processingResult in ProcessingResult(state)
            from startedAt in Arb.Generate<DateTime?>().Where(date => state >= CheckState.Running ? date != null : date == null)
            from performedAt in Arb.Generate<DateTime?>().Where(date => state >= CheckState.Running ? date != null : date == null)
            from completedAt in Arb.Generate<DateTime?>().Where(date => state >= CheckState.Complete ? date != null : date == null)
            select new CheckSpecimen
            {
                Id = id,
                UserId = userId,
                Type = type,
                Variant = variant,
                State = state,
                ExecutionContext = executionContext,
                ProcessingResult = processingResult,
                StartedAt = startedAt,
                PerformedAt = performedAt,
                CompletedAt = completedAt
            };

        private static Gen<CheckVariant> CheckVariants =>
            from id in Arb.Generate<Guid>()
            from provider in Arb.Generate<CheckProviderType>()
            select CheckVariant.Create(id, provider);

        private static Gen<CheckExecutionContext> ExecutionContext(CheckState state)
        {
            return
                from inputData in Dictionary.OrNull().Where(IsCorrectInputData)
                from externalData in ExternalData.OrNull()
                let executionContext = inputData != null ? CheckExecutionContext.Create(inputData, externalData) : null
                select executionContext;

            bool IsCorrectInputData(Dictionary<string, object> data) => state >= CheckState.Running ? data != null : data == null;
        }

        private static Gen<CheckProcessingResult> ProcessingResult(CheckState state)
        {
            return from result in Arb.Generate<CheckResult?>().Where(IsCorrectResult)
                   from decision in StringGenerators.NotEmpty(30).OrNull()
                   from outputData in CheckOutputDataGenerators.CheckOutputData.OrNull()
                   let processingResult = result.HasValue
                       ? CheckProcessingResult.Create(result.Value, decision, null)
                       : null
                   select processingResult;

            bool IsCorrectResult(CheckResult? result) => state != CheckState.Complete || result.HasValue;
        }

        private static Gen<Dictionary<string, object>> ExternalData =>
            from externalId in Arb.Generate<Guid>().Select(guid => guid.ToString())
            from externalData in Dictionary.Or(Gen.Constant(new Dictionary<string, object>()))
            select new Dictionary<string, object>(externalData) { [ExternalCheckProperties.ExternalId] = externalId };

        private static Gen<Dictionary<string, object>> Dictionary =>
            from amount in Gen.Choose(0, 5)
            from pairs in Gen.ArrayOf(amount, KeyValuePairs)
            let result = new Dictionary<string, object>(pairs)
            select result;

        private static Gen<KeyValuePair<string, object>> KeyValuePairs =>
            from key in StringGenerators.NotEmpty(10)
            from value in Arb.Generate<object>()
            where value is not char
            let result = KeyValuePair.Create(key, value)
            select result;
    }
}
