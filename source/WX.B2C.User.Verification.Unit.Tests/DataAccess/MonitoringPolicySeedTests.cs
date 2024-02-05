using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Seed;

namespace WX.B2C.User.Verification.Unit.Tests.DataAccess
{
    public class MonitoringPolicySeedTests
    {
        [Test]
        public void ShouldReadMonitoringPolicy()
        {
            var policies = SeedData.MonitoringPolicies;

            policies.Should().NotBeEmpty();
            policies.Should().HaveCount(6);

            policies.Should().Contain(policy => policy.Region == "GB" && policy.RegionType == RegionType.Country);
            policies.Should().Contain(policy => policy.Region == "EEA" && policy.RegionType == RegionType.Region);
            policies.Should().Contain(policy => policy.Region == "RoW" && policy.RegionType == RegionType.Region);
            policies.Should().Contain(policy => policy.Region == "APAC" && policy.RegionType == RegionType.Region);
            policies.Should().Contain(policy => policy.Region == "PH" && policy.RegionType == RegionType.Country);
            policies.Should().Contain(policy => policy.Region == "US" && policy.RegionType == RegionType.Country);

            policies.Should().OnlyHaveUniqueItems(policy => policy.Id);
        }

        [Test]
        public void ShouldReadingVerificationPolicyBeIndepotent()
        {
            var policies1 = SeedData.MonitoringPolicies;
            var policies2 = SeedData.MonitoringPolicies;

            policies1.Should().BeEquivalentTo(policies2);
        }

        [Test]
        public void ShouldReadTriggers()
        {
            var policies = SeedData.MonitoringPolicies;
            var monitoringPoliciesId = policies.Select(policy => policy.Id);

            var act = new Func<IReadOnlyCollection<TriggerVariant>>(() => SeedData.Triggers);

            var triggers = act.Should().NotThrow().Subject.Where(variant => monitoringPoliciesId.Contains(variant.PolicyId)).ToArray();
            triggers.Should().NotBeEmpty();
            triggers.SelectMany(trigger => trigger.Conditions).Should().NotBeEmpty();
            triggers.SelectMany(trigger => trigger.Commands).Should().NotBeEmpty();
        }


        [Test]
        public void ShouldTriggersHaveUniqueIds()
        {
            var triggers = SeedData.Triggers;

            triggers.Select(variant => variant.Id).Should().OnlyHaveUniqueItems();
        }
    }
}