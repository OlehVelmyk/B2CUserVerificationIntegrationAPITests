using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Automation.Services.Conditions;
using WX.B2C.User.Verification.Automation.Services.IoC;
using WX.B2C.User.Verification.DataAccess.Seed;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Unit.Tests.Conditions
{
    [TestFixture]
    public class RiskLevelConditionTests
    {
        private IConditionsFactory _factory;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterConditions();

            var container = builder.Build();
            _factory = container.Resolve<IConditionsFactory>();
        }

        /// <summary>
        /// This unit tests checks that data which we have in seed can be used for creating conditions and evaluating state. 
        /// </summary>
        [TestCase("64E483BC-20F6-4EEC-9F1A-2F2363CFC861", RiskLevel.High)]
        [TestCase("64E483BC-20F6-4EEC-9F1A-2F2363CFC861", RiskLevel.ExtraHigh)]
        public void ShouldBeSatisfied_WhenRiskLevelIsMatched(string triggerVariant, RiskLevel riskLevel)
        {
            var trigger = SeedData.Triggers.First(variant => variant.Id == Guid.Parse(triggerVariant));
            var context = new Dictionary<string, object>
            {
                { XPathes.RiskLevel, riskLevel },
            };

            var conditions = trigger.Conditions.Select(condition => _factory.Create(condition));
            var results = conditions.Select(condition => condition.IsSatisfied(context));

            results.Should().Contain(true);
        }

        /// <summary>
        /// This unit tests checks that data which we have in seed can be used for creating conditions and evaluating state. 
        /// </summary>
        [TestCase("64E483BC-20F6-4EEC-9F1A-2F2363CFC861", RiskLevel.Medium)]
        [TestCase("64E483BC-20F6-4EEC-9F1A-2F2363CFC861", RiskLevel.Low)]
        public void ShouldNotBeSatisfied_WhenRiskLevelIsNotMatched(string triggerVariant, RiskLevel riskLevel)
        {
            var trigger = SeedData.Triggers.First(variant => variant.Id == Guid.Parse(triggerVariant));
            var context = new Dictionary<string, object>
            {
                { XPathes.RiskLevel, riskLevel },
            };

            var conditions = trigger.Conditions.Select(condition => _factory.Create(condition));
            var results = conditions.Select(condition => condition.IsSatisfied(context));

            results.Should().AllBeEquivalentTo(false);
        }
    }
}