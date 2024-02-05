using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Automation
{
    internal class CheckRunTests : BaseComponentTest
    {
        private ApplicationFixture _applicationFixture;
        private ChecksFixture _checksFixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _checksFixture = Resolve<ChecksFixture>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<NotGlobalUserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: Complete check
        /// Given user 
        /// When complete check
        /// Then check is completed
        /// </summary>
        [TestCaseSource(nameof(CheckSource))] // TODO: WRXB-10812
        public async Task ShouldCompleteCheck(string variantId, CheckType type, CheckProviderType provider)
        {
            // Given
            const string Replay = "";
            var fsSeed = string.IsNullOrEmpty(Replay) 
                ? FsCheckSeed.Create(Arb.Generate<Rnd>().Sample()) 
                : FsCheckSeed.Parse(Replay);
            fsSeed.Dump();

            var seed = Arb.Generate<Seed>().Sample(fsSeed);
            var userInfo = Arb.Generate<NotGlobalUserInfo>().Sample(fsSeed);

            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Act
            Func<Task> completeCheck = () => _checksFixture.CompleteAsync(userId, new Guid(variantId), seed, null);

            // Assert
            await completeCheck.Should().NotThrowAsync();
        }

        private static IEnumerable<TestCaseData> CheckSource =>
            new[]
            {
                new TestCaseData("0E3B0522-8330-487F-A35C-38CC6022812A", CheckType.FaceDuplication, CheckProviderType.Onfido), // Video
                new TestCaseData("0822EFCF-5F04-4D10-A144-460E2040968E", CheckType.FaceDuplication, CheckProviderType.Onfido), // Photo
                new TestCaseData("3B43F013-5AC2-4BD9-96DE-20A683331BCF", CheckType.FacialSimilarity, CheckProviderType.Onfido), // Photo
                new TestCaseData("6D930727-5BCC-4014-A9C1-B08B85491D34", CheckType.FacialSimilarity, CheckProviderType.Onfido), // Video
                new TestCaseData("29AAC87B-3AD4-40E0-B34F-3685CA64805D", CheckType.FacialSimilarity, CheckProviderType.Onfido), // Confirmation by photo
                new TestCaseData("23714F13-CBF6-41A4-85C6-719991E6C3F3", CheckType.FacialSimilarity, CheckProviderType.Onfido), // Confirmation by video
                new TestCaseData("D6F2DAF9-3F8F-4335-A7B8-4FC383471A1D", CheckType.IdentityDocument, CheckProviderType.Onfido),
                new TestCaseData("63EAA27A-A6FC-43A8-93E4-0D7BFBD7CF23", CheckType.IdentityEnhanced, CheckProviderType.Onfido),
                new TestCaseData("BB30DACB-F8A0-477C-941A-FB0C71C0297A", CheckType.RiskListsScreening, CheckProviderType.PassFort),
                new TestCaseData("759DE7BC-0D76-4C53-8FE3-702C2F6DD2CE", CheckType.RiskListsScreening, CheckProviderType.PassFort),
                new TestCaseData("34A33DF0-B9B5-4205-9CC6-1F90BE10D313", CheckType.RiskListsScreening, CheckProviderType.LexisNexis),
                new TestCaseData("7147ADE3-A665-4B03-9B89-99008218C12F", CheckType.FraudScreening, CheckProviderType.LexisNexis),
                new TestCaseData("A9E0048B-0F6B-44F0-8A22-703DD86BA05E", CheckType.TaxResidence, CheckProviderType.System),
                new TestCaseData("779E3ED5-68E1-4C95-8E15-E6D8957820BC", CheckType.IpMatch, CheckProviderType.System), // By region with PoA
                new TestCaseData("FE22EBBC-DA71-43A9-883C-B15F0124F0E3", CheckType.IpMatch, CheckProviderType.System), // By country
                new TestCaseData("6431DD13-EF54-4D99-978E-E94067913B43", CheckType.IpMatch, CheckProviderType.System), // By country with PoA
                new TestCaseData("CA5D8609-897E-48E1-B9E3-82BA4F40E6FF", CheckType.IpMatch, CheckProviderType.System), // By state with PoA
                new TestCaseData("372929F1-8597-4E50-A5CE-5881377AF295", CheckType.NameAndDoBDuplication, CheckProviderType.System),
                new TestCaseData("62BA1CC3-8802-4C13-B2BA-BB6EFBDBEE1F", CheckType.IdDocNumberDuplication, CheckProviderType.System),
            };
    }
}
