using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Facade.Controllers.Public;

namespace WX.B2C.User.Verification.Unit.Tests.PublicApi
{
    [TestFixture]
    internal class ErrorCodesTests
    {
        [Theory]
        public void Defined_ShouldContainAllCoded()
        {
            var errorCodes = typeof(Constants.ErrorCodes).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Select(info => info.GetValue(null));

            Constants.ErrorCodes.Defined.Should().BeEquivalentTo(errorCodes);
        }
    }
}
