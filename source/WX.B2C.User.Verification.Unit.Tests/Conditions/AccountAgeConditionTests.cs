using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.Automation.Services.Conditions;
using WX.B2C.User.Verification.Automation.Services.IoC;
using WX.B2C.User.Verification.DataAccess.Seed;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Unit.Tests.Conditions
{
    [TestFixture]
    public class AccountAgeConditionTests
    {
        private IConditionsFactory _factory;
        private ISystemClock _systemClock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var builder = new ContainerBuilder();
            _systemClock = Substitute.For<ISystemClock>();
            builder.Register(_ => _systemClock).As<ISystemClock>();
            builder.RegisterConditions();

            var container = builder.Build();
            _factory = container.Resolve<IConditionsFactory>();
        }

        /// <summary>
        /// This unit tests checks that data which we have in seed can be used for creating conditions and evaluating state. 
        /// </summary>
        [TestCase("651F41FE-29F9-4F2A-979D-F425377DB305", RiskLevel.Low, "01.01.2000", "01.01.2003")]
        [TestCase("651F41FE-29F9-4F2A-979D-F425377DB305", RiskLevel.Medium, "01.01.2000", "01.01.2002")]
        [TestCase("651F41FE-29F9-4F2A-979D-F425377DB305", RiskLevel.High, "01.01.2000", "01.01.2001")]
        public void ShouldBeSatisfied_WhenRiskLevelIsMatched_AndAccountAgeMoreThanLimit(string triggerVariant,
                                                                                        RiskLevel riskLevel,
                                                                                        string createdAt,
                                                                                        string today)
        {
            _systemClock.GetDate().Returns(DateTime.ParseExact(today, "dd.MM.yyyy", null));

            var trigger = SeedData.Triggers.First(variant => variant.Id == Guid.Parse(triggerVariant));
            var context = new Dictionary<string, object>
            {
                { XPathes.RiskLevel, riskLevel },
                { XPathes.ProfileCreationDate, DateTime.ParseExact(createdAt, "dd.MM.yyyy", null) }
            };

            var conditions = trigger.Conditions.Select(condition => _factory.Create(condition));
            var results = conditions.Select(condition => condition.IsSatisfied(context));

            results.Should().Contain(true);
        }

        /// <summary>
        /// This unit tests checks that data which we have in seed can be used for creating conditions and evaluating state. 
        /// </summary>
        [TestCase("651F41FE-29F9-4F2A-979D-F425377DB305", RiskLevel.Low, "01.01.2000", "31.12.2002")]
        [TestCase("651F41FE-29F9-4F2A-979D-F425377DB305", RiskLevel.Medium, "01.01.2000", "31.12.2001")]
        [TestCase("651F41FE-29F9-4F2A-979D-F425377DB305", RiskLevel.High, "01.01.2000", "31.12.2000")]
        public void ShouldNotBeSatisfied_WhenRiskLevelIsMatched_AndAccountAgeLessThanLimit(string triggerVariant,
                                                                                           RiskLevel riskLevel,
                                                                                           string createdAt,
                                                                                           string today)
        {
            _systemClock.GetDate().Returns(DateTime.ParseExact(today, "dd.MM.yyyy", null));

            var trigger = SeedData.Triggers.First(variant => variant.Id == Guid.Parse(triggerVariant));
            var context = new Dictionary<string, object>
            {
                { XPathes.RiskLevel, riskLevel },
                { XPathes.ProfileCreationDate, DateTime.ParseExact(createdAt, "dd.MM.yyyy", null) }
            };

            var conditions = trigger.Conditions.Select(condition => _factory.Create(condition));
            var results = conditions.Select(condition => condition.IsSatisfied(context));

            results.Should().AllBeEquivalentTo(false);
        }
    }
}