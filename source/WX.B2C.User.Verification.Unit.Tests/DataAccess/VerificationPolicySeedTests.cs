using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Seed;

namespace WX.B2C.User.Verification.Unit.Tests.DataAccess
{
    public class VerificationPolicySeedTests
    {

        [Test]
        public void ShouldReadCheckVariants()
        {
            var policies = SeedData.ChecksVariants;

            policies.Should().NotBeEmpty();
            policies.Select(variants => variants.Id).Should().OnlyHaveUniqueItems();
        }
        
        [Test]
        public void ShouldReadVerificationPolicy()
        {
            var policies = SeedData.VerificationPolicies;

            policies.Should().NotBeEmpty();
            policies.Should().HaveCount(7);

            policies.Should().Contain(policy => policy.Region == "EEA" && policy.RegionType == RegionType.Region);
            policies.Should().Contain(policy => policy.Region == "Global" && policy.RegionType == RegionType.Global);
            policies.Should().Contain(policy => policy.Region == "GB" && policy.RegionType == RegionType.Country);
            policies.Should().Contain(policy => policy.Region == "APAC" && policy.RegionType == RegionType.Region);
            policies.Should().Contain(policy => policy.Region == "US" && policy.RegionType == RegionType.Country);
            policies.Should().Contain(policy => policy.Region == "RoW" && policy.RegionType == RegionType.Region);
            policies.Should().Contain(policy => policy.Region == "RU" && policy.RegionType == RegionType.Country);

            //Related entities must not be set
            using var assertionScope = new AssertionScope();
            foreach (var verificationPolicy in policies)
            {
                verificationPolicy.Tasks.Should().BeNull();
            }

            policies.Should().OnlyHaveUniqueItems(policy => policy.Id);
        }

        [Test]
        public void ShouldReadingVerificationPolicyBeIndepotent()
        {
            var policies1 = SeedData.VerificationPolicies;
            var policies2 = SeedData.VerificationPolicies;

            policies1.Should().BeEquivalentTo(policies2);
        }

        [Test]
        public void ShouldReadPolicyEntities()
        {
            var act = new Func<IReadOnlyCollection<VerificationPolicy>>(() => SeedData.VerificationPolicies);

            act.Should().NotThrow();
        }

        [Test]
        public void ShouldReadTasksEntities()
        {
            var act = new Func<IReadOnlyCollection<TaskVariant>>(() => SeedData.Tasks);

            var taskVariants = act.Should().NotThrow().Subject;
            taskVariants.Should().OnlyHaveUniqueItems(taskVariant => taskVariant.Id);
        }

        [Test]
        public void ShouldReadTasksTemplates()
        {
            var policyTasks = SeedData.PolicyTasks;
            var tasks = SeedData.Tasks;

            tasks.Select(variant => variant.Id)
                 .Except(policyTasks.Select(task => task.TaskVariantId))
                 .Should().NotBeEmpty();
        }

        [Test]
        public void ShouldTasksHaveUniqueId()
        {
            var tasks = SeedData.Tasks;
            
            var tasksIds = tasks.Select(variant => variant.Id);
            tasksIds.Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void ShouldReadCheckVariantsEntities()
        {
            var act = new Func<IReadOnlyCollection<PolicyCheckVariant>>(() => SeedData.ChecksVariants);

            act.Should().NotThrow();
        }

        [Test]
        public void ShouldReadPolicyTasks()
        {
            var act = new Func<IReadOnlyCollection<PolicyTask>>(() => SeedData.PolicyTasks);

            var tasks = act.Should().NotThrow().Subject;
            tasks.Should().NotBeEmpty();
        }

        [Test]
        public void ShouldReadTriggers()
        {
            var policies = SeedData.VerificationPolicies;
            var verificationPolicies = policies.Select(policy => policy.Id);

            var act = new Func<IReadOnlyCollection<TriggerVariant>>(() => SeedData.Triggers);

            var triggers = act.Should().NotThrow().Subject.Where(variant => verificationPolicies.Contains(variant.PolicyId)).ToArray();
            triggers.Should().NotBeEmpty();
            triggers.SelectMany(trigger => trigger.Conditions).Should().NotBeEmpty();
            triggers.SelectMany(trigger => trigger.Commands).Should().NotBeEmpty();
        }
    }
}