using System.Collections.Generic;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Unit.Tests.Validators
{
    internal class OnlyUniqueValuesValidatorTests
    {
        private class TestModel
        {
            public IEnumerable<string> Values { get; set; }
        }

        private class TestModelValidator : AbstractValidator<TestModel>
        {
            public TestModelValidator()
            {
                RuleFor(model => model.Values)
                    .OnlyUniqueValues();
            }
        }

        [Test]
        public void ShouldNotHaveValidationError()
        {
            var validator = new TestModelValidator();

            var testModel = new TestModel
            {
                Values = new[] { "1", "2", "3", "4" }
            };

            var result = validator.TestValidate(testModel);

            result.ShouldNotHaveValidationErrorFor(model => model.Values);
        }

        [Test]
        public void ShouldHaveValidationError()
        {
            var validator = new TestModelValidator();

            var testModel = new TestModel
            {
                Values = new[] { "1", "1", "2", "2", "3", "4" }
            };

            var result = validator.TestValidate(testModel);

            result.ShouldHaveValidationErrorFor(model => model.Values)
                  .WithErrorMessage("Values must only contain unique values. Duplicate values are [1,2].");
        }
    }
}
