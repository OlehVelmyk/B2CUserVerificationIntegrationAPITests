using System;
using System.Linq;
using FsCheck;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Utilities;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class VerificationTaskNoRelationsArbitrary : Arbitrary<VerificationTaskNoRelationsSpecimen>
    {
        public static Arbitrary<VerificationTaskNoRelationsSpecimen> Create() => new VerificationTaskNoRelationsArbitrary();

        public override Gen<VerificationTaskNoRelationsSpecimen> Generator =>
            from specimen in Arb.Generate<VerificationTaskSpecimen>()
            select new VerificationTaskNoRelationsSpecimen(specimen);
    }

    internal class VerificationTaskArbitrary : Arbitrary<VerificationTaskSpecimen>
    {
        public override Gen<VerificationTaskSpecimen> Generator =>
            from id in Arb.Generate<Guid>()
            from userId in Arb.Generate<Guid>()
            from variantId in Arb.Generate<Guid>()
            from taskType in Arb.Generate<TaskType>()
            from creationDate in Arb.Generate<DateTime>()
            from taskResult in Arb.Generate<TaskResult?>().OrNull()
            from taskState in GenerateStateFromResult(taskResult)
            from acceptanceChecksCount in Gen.Choose(0, 5)
            from acceptanceChecks in Gen.ArrayOf(acceptanceChecksCount, Arb.Generate<TaskCheck>())
            from checksCount in Gen.Choose(0, acceptanceChecksCount)
            from performedChecks in Gen.ArrayOf(checksCount, Gen.Elements(acceptanceChecks))
            from collectionStepsCount in Gen.Choose(0, 5)
            from collectionSteps in Gen.ArrayOf(collectionStepsCount, Arb.Generate<TaskCollectionStep>())
            let acceptanceCheckIds = acceptanceChecks.Select(x => x.Id).ToArray()
            select new VerificationTaskSpecimen
            { 
                Id = id,
                UserId = userId,
                VariantId = variantId,
                Type = taskType,
                CreationDate = creationDate,
                State = taskState,
                Result = taskResult,
                CollectionSteps = collectionSteps.ToHashSet(),
                PerformedChecks = performedChecks.ToHashSet(),
                ExpirationDetails = null
            };

        public static Arbitrary<VerificationTaskSpecimen> Create() => new VerificationTaskArbitrary();

        private static Gen<TaskState> GenerateStateFromResult(TaskResult? taskResult)
        {
            var state = taskResult.HasValue ? TaskState.Completed : TaskState.Incomplete;
            return Gen.Constant(state);
        }
    }

    internal class TaskCollectionStepArbitrary : Arbitrary<TaskCollectionStep>
    {
        public static Arbitrary<TaskCollectionStep> Create() => new TaskCollectionStepArbitrary();

        public override Gen<TaskCollectionStep> Generator =>
            from id in Arb.Generate<Guid>()
            from state in Arb.Generate<CollectionStepState>()
            from isRequired in Arb.Generate<bool>()
            select new TaskCollectionStep(id, state, isRequired);
    }

    internal class TaskCheckArbitrary : Arbitrary<TaskCheck>
    {
        public static Arbitrary<TaskCheck> Create() => new TaskCheckArbitrary();

        public override Gen<TaskCheck> Generator =>
            from id in Arb.Generate<Guid>()
            from variantId in Arb.Generate<Guid>()
            from type in Arb.Generate<CheckType>()
            from state in Arb.Generate<CheckState>()
            from result in Arb.Generate<CheckResult?>().OrNull()
            where state is CheckState.Complete ? result.HasValue : !result.HasValue
            select new TaskCheck(id, variantId, type, state, result);
    }
}