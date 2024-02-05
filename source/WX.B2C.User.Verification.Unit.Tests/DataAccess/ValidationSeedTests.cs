using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.DataAccess.Seed;

namespace WX.B2C.User.Verification.Unit.Tests.DataAccess
{
    public class ValidationSeedTests
    {
        [Test]
        public void ShouldReadValidationRules()
        {
            var rules = SeedData.ValidationRules;

            rules.Should().NotBeEmpty();
            rules.Select(variants => variants.Id).Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void ShouldReadValidationPolicies()
        {
            var policies = SeedData.ValidationPolicies;

            policies.Should().NotBeEmpty();
            policies.Select(policy => policy.Id).Should().OnlyHaveUniqueItems();
            policies.Select(policy => policy.Region + policy.RegionType).Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void ShouldReadPolicyValidationRules()
        {

            var existingPolicies = SeedData.ValidationPolicies.Select(policy => policy.Id);
            var existingRules = SeedData.ValidationRules.Select(rule => rule.Id);

            var policies = SeedData.PolicyValidationRules;

            policies.Should().NotBeEmpty();
            policies.Select(policy => $"{policy.ValidationPolicyId}:{policy.ValidationRuleId}").Should().OnlyHaveUniqueItems();

            existingRules.Should().Contain(policies.Select(rule => rule.ValidationRuleId).Distinct());
            existingPolicies.Should().Contain(policies.Select(rule => rule.ValidationPolicyId).Distinct());
        }
        
        [Test]
        public void ShouldReadTaskCheckVariants()
        {
            var existingTasks = SeedData.Tasks.Select(task => task.Id);
            var existingCheckVariants = SeedData.ChecksVariants.Select(checkVariant => checkVariant.Id);

            var taskCheckVariants = SeedData.TaskCheckVariants;

            taskCheckVariants.Should().NotBeEmpty();
            taskCheckVariants.Select(taskCheckVariant => $"{taskCheckVariant.TaskId}:{taskCheckVariant.CheckVariantId}").Should().OnlyHaveUniqueItems();

            existingCheckVariants.Should().Contain(taskCheckVariants.Select(rule => rule.CheckVariantId).Distinct());
            existingTasks.Should().Contain(taskCheckVariants.Select(rule => rule.TaskId).Distinct());
        }
    }
}