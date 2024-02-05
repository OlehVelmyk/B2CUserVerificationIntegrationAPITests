using System.Collections.Generic;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Automation.Services.Conditions;
using WX.B2C.User.Verification.Automation.Services.IoC;
using WX.B2C.User.Verification.Core.Contracts.Conditions;

namespace WX.B2C.User.Verification.Unit.Tests.Conditions
{
    [TestFixture]
    public class MatchCountryConditionTests
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

        [Test]
        public void ShouldBeSatisfied_WhenCountryIsMatched()
        {
            var country = "Test";
            var condition = _factory.Create(new Condition
            {
                Type = ConditionType.MatchCountry,
                Value = country
            });
            var context = new Dictionary<string, object>
            {
                { MatchCountryCondition.MatchCountryRequiredData, new[] { country } },
            };

            var result = condition.IsSatisfied(context);

            result.Should().BeTrue();
        }

        [Test]
        public void ShouldNotBeSatisfied_WhenCountryIsNotMatched()
        {
            var country = "Test";
            var condition = _factory.Create(new Condition
            {
                Type = ConditionType.MatchCountry,
                Value = country
            });
            var context = new Dictionary<string, object>
            {
                { MatchCountryCondition.MatchCountryRequiredData, new[] { country + "~" } },
            };

            var result = condition.IsSatisfied(context);

            result.Should().BeFalse();
        }
    }
}