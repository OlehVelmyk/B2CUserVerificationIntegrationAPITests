using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Processors.Validators;

namespace WX.B2C.User.Verification.Unit.Tests.Validators
{
    internal class KnownFacesReportValidatorTests
    {
        private readonly KnownFacesReportValidator _validator;

        public KnownFacesReportValidatorTests()
        {
            _validator = new KnownFacesReportValidator();
        }

        [Test]
        [TestCaseSource(nameof(ClearTestCases))]
        public void ShouldNotFail(KnownFacesReport report)
        {
            // Act
            var validationResult = _validator.TestValidate(report);

            // Assert
            validationResult.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [TestCaseSource(nameof(FraudTestCases))]
        [TestCaseSource(nameof(ResubmitTestCases))]
        public void ShouldFail(KnownFacesReport report, string expectedDecision)
        {
            // Act
            var validationResult = _validator.TestValidate(report);

            // Assert
            validationResult.ShouldHaveAnyValidationError();
            validationResult.Errors.Should().HaveCount(1);

            var error = validationResult.Errors[0];
            error.ErrorCode.Should()
                 .Be(expectedDecision);
            error.CustomState.Should()
                 .BeOfType<string[]>()
                 .Which.Should()
                 .NotBeEmpty();
        }

        private static IEnumerable<object> ClearTestCases => KnownFacesReportValidatorTestCases.ClearTestCases;

        private static IEnumerable<object> FraudTestCases =>
            KnownFacesReportValidatorTestCases.FraudTestCases.Select(report => new object[] { report, CheckDecisions.Fraud });

        private static IEnumerable<object> ResubmitTestCases =>
            KnownFacesReportValidatorTestCases.ResubmitTestCases.Select(report => new object[] { report, CheckDecisions.Resubmit });
    }
}
