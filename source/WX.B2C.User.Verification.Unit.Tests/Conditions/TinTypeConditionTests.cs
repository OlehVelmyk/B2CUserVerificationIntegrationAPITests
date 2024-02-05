using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Automation.Services.Conditions;
using WX.B2C.User.Verification.Automation.Services.IoC;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.DataAccess.Seed;
using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Unit.Tests.Conditions
{
    [TestFixture]
    public class TinTypeConditionTests
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
        [TestCase("44FE4C45-7F6D-48A8-8690-E5D92C97F61B", TinType.ITIN)]
        public void ShouldBeSatisfied_WhenTinTypeIsMatched(string triggerVariant, TinType tinType)
        {
            var trigger = SeedData.Triggers.First(variant => variant.Id == Guid.Parse(triggerVariant));
            var context = new Dictionary<string, object>
            {
                { XPathes.Tin, new TinDto {Type = tinType} },
            };

            var conditions = trigger.Conditions.Select(condition => _factory.Create(condition)).OfType<TinTypeCondition>();
            var results = conditions.Select(condition => condition.IsSatisfied(context));

            results.Should().Contain(true);
        }

        /// <summary>
        /// This unit tests checks that data which we have in seed can be used for creating conditions and evaluating state. 
        /// </summary>
        [TestCase("44FE4C45-7F6D-48A8-8690-E5D92C97F61B", TinType.SSN)]
        public void ShouldNotBeSatisfied_WhenTinTypeIsNotMatched(string triggerVariant, TinType tinType)
        {
            var trigger = SeedData.Triggers.First(variant => variant.Id == Guid.Parse(triggerVariant));
            var context = new Dictionary<string, object>
            {
                { XPathes.Tin, new TinDto {Type = tinType} },
            };

            var conditions = trigger.Conditions.Select(condition => _factory.Create(condition)).OfType<TinTypeCondition>(); ;
            var results = conditions.Select(condition => condition.IsSatisfied(context));

            results.Should().AllBeEquivalentTo(false);
        }
    }
}