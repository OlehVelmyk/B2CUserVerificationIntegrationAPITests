using System;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Unit.Tests.Services
{
    [TestFixture]
    public class XPathTests
    {
        [Test]
        public void ShouldBeEqualToString()
        {
            var xPathString = "VerificationDetails.Tin";
            var xPath = XPathes.Tin;

            var equalsResult = xPath.Equals(xPathString);
            var reversedEqualsResult = xPathString.Equals(xPath);
            var equalsOperatorResult = xPathString == xPath;
            var reversedEqualsOperatorResult = xPath == xPathString;


            equalsResult.Should().BeTrue();
            reversedEqualsResult.Should().BeTrue();
            equalsOperatorResult.Should().BeTrue();
            reversedEqualsOperatorResult.Should().BeTrue();
        }

        [Test]
        public void ShouldCreateSurveyXPathWithUpperCaseTemplateId()
        {
            var templateId = Guid.NewGuid();
            var xPathString = $"Survey.{templateId.ToString().ToUpper()}";
            var xPath = new SurveyXPath(templateId);

            var equalsResult = xPath.Equals(xPathString);
            var reversedEqualsResult = xPathString.Equals(xPath);
            var equalsOperatorResult = xPathString == xPath;
            var reversedEqualsOperatorResult = xPath == xPathString;


            equalsResult.Should().BeTrue();
            reversedEqualsResult.Should().BeTrue();
            equalsOperatorResult.Should().BeTrue();
            reversedEqualsOperatorResult.Should().BeTrue();
        }
    }
}
