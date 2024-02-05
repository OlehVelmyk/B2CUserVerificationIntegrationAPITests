using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using WX.B2C.User.Verification.DataAccess.Seed.Models;

namespace WX.B2C.User.Verification.DataAccess.Seed.Validators
{
    /// <summary>
    /// Validate that verification policy is correctly setup.
    /// NOTE: instance has state, therefore need to be create one instance per one policy
    /// </summary>
    internal class VerificationPolicyValidator : BaseSeedValidator<VerificationPolicy>
    {
        private const string MustBeUniqueSuffix = " must be unique";
        private const string IdMustBeUniqueSuffix = nameof(Task.Id) + MustBeUniqueSuffix;
        private const string TypeMustBeUniqueSuffix = nameof(Task.Type) + MustBeUniqueSuffix;
        private const string CheckVariantNotFound = nameof(CheckVariant) + " :{PropertyValue} is not found";

        public VerificationPolicyValidator()
        {
            RuleFor(policy => policy.Region).NotEmpty();
            RuleFor(policy => policy.RegionType).NotEmpty();
            RuleFor(policy => policy.Id).NotEmpty();
            RuleFor(policy => policy.Name).NotEmpty();
            RuleForEach(policy => policy.Tasks).SetValidator(new TaskValidator());

            RuleFor(policy => policy.Tasks.Select(task => task.Id)).Must(BeUnique).WithMessage(nameof(Task) + IdMustBeUniqueSuffix);

            //TODO WRXB-9629 restore is needed 
            ////RuleFor(policy => policy.Tasks.Select(task => task.Type)).Must(BeUnique).WithMessage(nameof(Task) + TypeMustBeUniqueSuffix);


            RuleForEach(policy => policy.Tasks.SelectMany(task => ArrayOrEmpty(task.ChecksVariants)))
                .Must(ChecksExists)
                .OverridePropertyName("TaskCheck")
                .WithMessage(CheckVariantNotFound);
        }

        private static T[] ArrayOrEmpty<T>(T[] array)
        {
            return array ?? Array.Empty<T>();
        }

        private bool BeUnique<T>(IEnumerable<T> collection)
        {
            var elements = collection.ToArray();
            return elements.Distinct().Count() == elements.Length;
        }

        private bool ChecksExists(VerificationPolicy policy, Guid checkVariantId)
        {
            var actualCheckTypes = policy.PossibleCheckVariants.Select(check => check.Id);
            return actualCheckTypes.Contains(checkVariantId);
        }
    }
}