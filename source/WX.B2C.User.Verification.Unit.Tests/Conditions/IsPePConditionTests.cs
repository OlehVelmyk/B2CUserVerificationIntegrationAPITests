using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Automation.Services.Conditions;
using WX.B2C.User.Verification.Automation.Services.IoC;
using WX.B2C.User.Verification.Core.Contracts.Conditions;
using WX.B2C.User.Verification.DataAccess;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.B2C.User.Verification.DataAccess.Seed;
using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Unit.Tests.Conditions
{
    [TestFixture]
    public class IsPePConditionTests
    {
        private IConditionsFactory _factory;
        private IPolicyObjectsDeserializer _policyDeserializer;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterConditions().RegisterDataAccess();

            var container = builder.Build();
            _factory = container.Resolve<IConditionsFactory>();
            _policyDeserializer = container.Resolve<IPolicyObjectsDeserializer>();
        }

        /// <summary>
        /// This unit tests checks that data which we have in seed can be used for creating conditions and evaluating state. 
        /// </summary>
        [TestCase("44FE4C45-7F6D-48A8-8690-E5D92C97F61B", true)]
        public void ShouldBeSatisfied_WhenUserIsPep(string triggerVariant, bool isPep)
        {
            var trigger = SeedData.Triggers.First(variant => variant.Id == Guid.Parse(triggerVariant));
            var context = new Dictionary<string, object>
            {
                { XPathes.IsPep, isPep },
            };

            var conditions = trigger.Conditions.Select(condition => _factory.Create(condition)).OfType<IsPePCondition>();
            var results = conditions.Select(condition => condition.IsSatisfied(context));

            results.Should().Contain(true);
        }

        /// <summary>
        /// This unit tests checks that data which we have in seed can be used for creating conditions and evaluating state. 
        /// </summary>
        [TestCase("44FE4C45-7F6D-48A8-8690-E5D92C97F61B", false)]
        public void ShouldNotBeSatisfied_WhenUserIsNotPep(string triggerVariant, bool isPep)
        {
            var trigger = SeedData.Triggers.First(variant => variant.Id == Guid.Parse(triggerVariant));
            var context = new Dictionary<string, object>
            {
                { XPathes.IsPep, isPep },
            };

            var conditions = trigger.Conditions.Select(condition => _factory.Create(condition)).OfType<IsPePCondition>();
            var results = conditions.Select(condition => condition.IsSatisfied(context));

            results.Should().AllBeEquivalentTo(false);
        }

        /// <summary>
        /// This unit tests checks that data which we have in seed can be used for creating conditions and evaluating state. 
        /// </summary>
        [TestCase("759DE7BC-0D76-4C53-8FE3-702C2F6DD2CE", true)]
        public void FailResult_ShouldBeSatisfied_WhenUserIsPep(string checkVariant, bool isPep)
        {
            var check = SeedData.ChecksVariants.First(variant => variant.Id == Guid.Parse(checkVariant));
            var context = new Dictionary<string, object>
            {
                { VerificationProperty.IsPep, isPep },
            };

            var conditionInfo = _policyDeserializer.Deserialize<Condition>(check.FailResultCondition);
            var condition = _factory.Create(conditionInfo);

            var result = condition.IsSatisfied(context);

            condition.Should().BeOfType<IsPePCondition>();
            result.Should().BeTrue();
        }

        /// <summary>
        /// This unit tests checks that data which we have in seed can be used for creating conditions and evaluating state. 
        /// </summary>
        [TestCase("759DE7BC-0D76-4C53-8FE3-702C2F6DD2CE", false)]
        public void FailResult_ShouldNotBeSatisfied_WhenUserIsNotPep(string checkVariant, bool isPep)
        {
            var check = SeedData.ChecksVariants.First(variant => variant.Id == Guid.Parse(checkVariant));
            var context = new Dictionary<string, object>
            {
                { VerificationProperty.IsPep, isPep },
            };

            var conditionInfo = _policyDeserializer.Deserialize<Condition>(check.FailResultCondition);
            var condition = _factory.Create(conditionInfo);

            var result = condition.IsSatisfied(context);

            condition.Should().BeOfType<IsPePCondition>();
            result.Should().BeFalse();
        }
    }
}