using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using WX.B2C.User.Verification.Automation.Services.Validators;

namespace WX.B2C.User.Verification.Unit.Tests.Events
{
    [TestFixture]
    internal class ApplicationRejectionReasonsSyncTests
    {
        [Test]
        public void ApplicationRejectionReasons_ShouldBeInSyncInCoreAndEvents()
        {
            var eventsConstantType = typeof(WX.B2C.User.Verification.Events.EventArgs.ApplicationRejectionReasons);
            var coreConstantsType = typeof(ApplicationRejectionReasons);

            var eventConstants = GetConstants(eventsConstantType);
            var coreConstants = GetConstants(coreConstantsType);

            eventConstants.Length.Should().Be(coreConstants.Length);

            using (new AssertionScope())
            {
                foreach (var coreConstant in coreConstants)
                {
                    var actualField = eventConstants.FirstOrDefault(info => info.Name == coreConstant.Name);
                    actualField.Should().NotBeNull($"Cannot find constant {coreConstant.Name}");

                    var expected = coreConstant.GetValue(null);
                    var actual = actualField.GetValue(null);

                    expected.Should().Be(actual, $"Value should be equal for {coreConstant.Name}");
                }
            }
            
            FieldInfo[] GetConstants(Type type) =>
                type.GetFields(BindingFlags.Public | BindingFlags.Static |
                               BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                    .ToArray();
        }
    }
}
