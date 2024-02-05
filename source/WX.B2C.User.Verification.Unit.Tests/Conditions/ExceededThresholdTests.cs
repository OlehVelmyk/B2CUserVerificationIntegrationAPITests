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
    public class ExceededThresholdConditionTests
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
        [TestCase("044529B5-D983-4F57-8DBA-16FEBF264052", RiskLevel.Low, 60_000)]
        [TestCase("044529B5-D983-4F57-8DBA-16FEBF264052", RiskLevel.Low, 60_001)]
        [TestCase("044529B5-D983-4F57-8DBA-16FEBF264052", RiskLevel.Medium, 30_000)]
        [TestCase("044529B5-D983-4F57-8DBA-16FEBF264052", RiskLevel.Medium, 30_001)]
        public void ShouldBeSatisfied_WhenRiskLevelIsMatched_AndTurnoverMoreThanThreshold(string triggerVariant,
                                                                                          RiskLevel riskLevel,
                                                                                          decimal turnover)
        {
            var trigger = SeedData.Triggers.First(variant => variant.Id == Guid.Parse(triggerVariant));
            var context = new Dictionary<string, object>
            {
                { XPathes.RiskLevel, riskLevel },
                { XPathes.Turnover, turnover }
            };

            var conditions = trigger.Conditions.Select(condition => _factory.Create(condition));
            var results = conditions.Select(condition => condition.IsSatisfied(context));

            results.Should().Contain(true);
        }

        /// <summary>
        /// This unit tests checks that data which we have in seed can be used for creating conditions and evaluating state. 
        /// </summary>
        [TestCase("044529B5-D983-4F57-8DBA-16FEBF264052", RiskLevel.Low, 24_999.999)]
        [TestCase("044529B5-D983-4F57-8DBA-16FEBF264052", RiskLevel.Low, 0)]
        [TestCase("044529B5-D983-4F57-8DBA-16FEBF264052", RiskLevel.Medium, 14_999.999)]
        [TestCase("044529B5-D983-4F57-8DBA-16FEBF264052", RiskLevel.Medium, 0)]
        public void ShouldNotBeSatisfied_WhenRiskLevelIsMatched_AndTurnoverLessThanThreshold(string triggerVariant,
            RiskLevel riskLevel,
            decimal turnover)
        {
            var trigger = SeedData.Triggers.First(variant => variant.Id == Guid.Parse(triggerVariant));
            var context = new Dictionary<string, object>
            {
                { XPathes.RiskLevel, riskLevel },
                { XPathes.Turnover, turnover }
            };

            var conditions = trigger.Conditions.Select(condition => _factory.Create(condition));
            var results = conditions.Select(condition => condition.IsSatisfied(context));

            results.Should().AllBeEquivalentTo(false);
        }

        /// <summary>
        /// This unit tests checks that data which we have in seed can be used for creating conditions and evaluating state. 
        /// </summary>
        [TestCase("044529B5-D983-4F57-8DBA-16FEBF264052", RiskLevel.High, 99_999.999)]
        [TestCase("044529B5-D983-4F57-8DBA-16FEBF264052", RiskLevel.ExtraHigh, 99_999.999)]
        public void ShouldNotBeSatisfied_WhenRiskLevelIsNotMatched(string triggerVariant,
                                                                   RiskLevel riskLevel,
                                                                   decimal turnover)
        {
            var trigger = SeedData.Triggers.First(variant => variant.Id == Guid.Parse(triggerVariant));
            var context = new Dictionary<string, object>
            {
                { XPathes.RiskLevel, riskLevel },
                { XPathes.Turnover, turnover }
            };

            var conditions = trigger.Conditions.Select(condition => _factory.Create(condition));
            var results = conditions.Select(condition => condition.IsSatisfied(context));

            results.Should().AllBeEquivalentTo(false);
        }
    }
}