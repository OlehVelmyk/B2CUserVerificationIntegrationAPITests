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
    internal class FacialSimilarityReportValidatorTests
    {
        private readonly FacialSimilarityReportValidator _validator;

        public FacialSimilarityReportValidatorTests()
        {
            _validator = new FacialSimilarityReportValidator();
        }

        [Test]
        [TestCaseSource(nameof(ClearTestCases))]
        public void ShouldNotFail(Report facialSimilarityReport)
        {
            // Act
            var validationResult = _validator.TestValidate(facialSimilarityReport);

            // Assert
            validationResult.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [TestCaseSource(nameof(ResubmitTestCases))]
        [TestCaseSource(nameof(ConsiderTestCases))]
        public void ShouldFail(Report facialSimilarityReport, string expectedDecision)
        {
            // Act
            var validationResult = _validator.TestValidate(facialSimilarityReport);

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

        private static IEnumerable<object> ClearTestCases => FacialSimilarityReportValidatorTestCases.ClearTestCases;

        private static IEnumerable<object> ResubmitTestCases =>
            FacialSimilarityReportValidatorTestCases.ResubmitTestCases.Select(report => new object[] { report, CheckDecisions.Resubmit });

        private static IEnumerable<object> ConsiderTestCases =>
            FacialSimilarityReportValidatorTestCases.ConsiderTestCases.Select(report => new object[] { report, CheckDecisions.Consider });
    }
}
