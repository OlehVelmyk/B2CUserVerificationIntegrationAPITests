using System;
using System.Linq;
using FsCheck;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Utilities;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class ApplicationNoRelationsArbitrary : Arbitrary<ApplicationNoRelationsSpecimen>
    {
        public static Arbitrary<ApplicationNoRelationsSpecimen> Create() => new ApplicationNoRelationsArbitrary();

        public override Gen<ApplicationNoRelationsSpecimen> Generator =>
            from specimen in Arb.Generate<ApplicationSpecimen>()
            select new ApplicationNoRelationsSpecimen(specimen);
    }

    internal class ApplicationArbitrary : Arbitrary<ApplicationSpecimen>
    {
        public static Arbitrary<ApplicationSpecimen> Create() => new ApplicationArbitrary();

        public override Gen<ApplicationSpecimen> Generator =>
            from id in Arb.Generate<Guid>()
            from userId in Arb.Generate<Guid>()
            from policyId in Arb.Generate<Guid>()
            from productType in Arb.Generate<ProductType>()
            from state in Arb.Generate<ApplicationState>()
            from previousState in Arb.Generate<ApplicationState?>().OrNull()
            from amountOfDecisons in Gen.Choose(0, 5)
            from decision in Gen.ArrayOf(amountOfDecisons, Arb.Generate<NonEmptyString>())
            from amountOfTasks in Gen.Choose(0, 5)
            from requiredTasks in Gen.ArrayOf(amountOfTasks, Arb.Generate<VerificationTaskSpecimen>()
                                                                .Override(userId, t => t.UserId))
            select new ApplicationSpecimen
            {
                Id = id,
                UserId = userId,
                PolicyId = policyId,
                ProductType = productType,
                State = state,
                PreviousState = previousState,
                DecisionReasons = decision.Select(x => x.Item).ToHashSet(),
                RequiredTasks = requiredTasks
            };
    }

    internal class ApplicationChangelogArbitrary : Arbitrary<ApplicationStateChangelogDto>
    {
        public static Arbitrary<ApplicationStateChangelogDto> Create() => new ApplicationChangelogArbitrary();

        public override Gen<ApplicationStateChangelogDto> Generator =>
            from applicationId in Arb.Generate<Guid>()
            from firstApprovedDate in Arb.Generate<DateTime?>().OrNull()
            from lastApprovedDate in Arb.Generate<DateTime?>().OrNull()
            where firstApprovedDate.HasValue ? lastApprovedDate >= firstApprovedDate : !lastApprovedDate.HasValue
            select new ApplicationStateChangelogDto
            {
                ApplicationId = applicationId,
                FirstApprovedDate = firstApprovedDate,
                LastApprovedDate = lastApprovedDate
            };
    }
}
