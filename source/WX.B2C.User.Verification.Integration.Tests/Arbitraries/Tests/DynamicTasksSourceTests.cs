using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using WX.B2C.User.Verification.DataAccess.Seed;
using WX.B2C.User.Verification.Integration.Tests.Models;
using static WX.B2C.User.Verification.Integration.Tests.Constants;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.Tests
{
    [TestFixture]
    internal class DynamicTasksSourceTests
    {
        [Test]
        public void ShouldBeSyncWithDbSeed()
        {
            var dbPolicyTasks = SeedData.PolicyTasks;
            var dbTasks = SeedData.Tasks;

            var dbTempateTasks = dbTasks.Select(variant => variant.Id)
                                        .Except(dbPolicyTasks.Select(task => task.TaskVariantId))
                                        .Select(variantId => dbTasks.First(tv => tv.Id == variantId))
                                        .ToArray();

            var dynamicTasksPath = Path.Combine(Constants.RootPath, Arbitrary.SourcesFolder, Arbitrary.DynamicTasks);
            var json = File.ReadAllText(dynamicTasksPath);
            var testTemplateTasks = JsonConvert.DeserializeObject<PolicyTaskSpecimen[]>(json, new JsonSerializerSettings { Converters = { new StringEnumConverter() } });

            testTemplateTasks.Should().HaveSameCount(dbTempateTasks);
            foreach(var dbTempateTask in dbTempateTasks)
            {
                var testTemplateTask = testTemplateTasks.SingleOrDefault(test => test.TaskVariantId == dbTempateTask.Id);
                testTemplateTask.Should().NotBeNull();
                testTemplateTask.TaskType.Should().Be(dbTempateTask.Type);                
            }
        }
    }
}
