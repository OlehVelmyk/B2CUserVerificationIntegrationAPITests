using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.DataAccess.Seed;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks;

namespace WX.B2C.User.Verification.Unit.Tests.Jobs
{
    [TestFixture]
    internal class HardcodedDynamicTasksProviderTests
    {
        private readonly HardcodedDynamicTasksProvider _sut;

        public HardcodedDynamicTasksProviderTests()
        {
            _sut = new HardcodedDynamicTasksProvider();
        }

        [Test]
        /// <summary>
        /// Test that all template (dynamic) tasks are presented in HardcodedDynamicTasksProvider
        /// but it cannot test that tasks are located in appropriate policy
        /// </summary>
        public void ShouldBeSyncWithDbSeed()
        {
            // Arrange
            var dbPolicyTasks = SeedData.PolicyTasks;
            var dbTasks = SeedData.Tasks;

            var dbTempateTasks = dbTasks.Select(variant => variant.Id)
                                        .Except(dbPolicyTasks.Select(task => task.TaskVariantId))
                                        .Select(variantId => dbTasks.First(tv => tv.Id == variantId))
                                        .ToArray();

            // Act
            var templateTasks = SeedData.VerificationPolicies.Select(policy => _sut.Get(policy.Id)).Flatten();

            // Assert
            templateTasks.Should().HaveSameCount(dbTempateTasks);
            foreach (var dbTempateTask in dbTempateTasks)
            {
                var testTemplateTask = templateTasks.SingleOrDefault(template => template.Id == dbTempateTask.Id);
                testTemplateTask.Should().NotBeNull();
                testTemplateTask.Type.Should().Be(dbTempateTask.Type);
            }
        }
    }
}
