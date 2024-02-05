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
    internal class DocumentReportValidatorTests
    {
        private readonly DocumentReportValidator _validator;

        public DocumentReportValidatorTests()
        {
            _validator = new DocumentReportValidator();
        }

        [Test]
        [TestCaseSource(nameof(ClearTestCases))]
        public void ShouldNotFail(DocumentReport documentReport)
        {
            // Act
            var validationResult = _validator.TestValidate(documentReport);

            // Assert
            validationResult.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [TestCaseSource(nameof(ResubmitTestCases))]
        [TestCaseSource(nameof(FraudTestCases))]
        public void ShouldFail(DocumentReport documentReport, string expectedDecision)
        {
            // Act
            var validationResult = _validator.TestValidate(documentReport);

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

        private static IEnumerable<object> ClearTestCases => DocumentReportValidatorTestCases.ClearTestCases;

        private static IEnumerable<object> ResubmitTestCases =>
            DocumentReportValidatorTestCases.ResubmitTestCases.Select(report => new object[] { report, CheckDecisions.Resubmit });

        private static IEnumerable<object> FraudTestCases =>
            DocumentReportValidatorTestCases.FraudTestCases.Select(report => new object[] { report, CheckDecisions.Fraud });
    }
}
