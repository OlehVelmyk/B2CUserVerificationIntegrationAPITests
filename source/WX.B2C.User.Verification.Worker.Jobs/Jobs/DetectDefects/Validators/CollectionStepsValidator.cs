using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;
using CollectionStep = WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models.CollectionStep;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators
{
    internal class CollectionStepsValidator : AbstractValidator<UserConsistency>
    {
        private static Dictionary<string, Func<ProfileDataExistence, bool>> DataExistenceMapping =
            new()
            {
                { XPathes.FullName, dataExistence => dataExistence.FullName},                         
                { XPathes.Birthdate, dataExistence => dataExistence.DateOfBirth},                     
                { XPathes.ResidenceAddress, dataExistence => dataExistence.Address},                  
                { XPathes.IpAddress, dataExistence => dataExistence.IpAddress},                       
                { XPathes.TaxResidence, dataExistence => dataExistence.TaxResidence},                 
                { XPathes.VerifiedNationality, dataExistence => dataExistence.Nationality},           
                { XPathes.Tin, dataExistence => dataExistence.Tin},                                   
                { XPathes.IdDocumentNumber, dataExistence => dataExistence.IdDocumentNumber},         
                { XPathes.ProofOfIdentityDocument, dataExistence => dataExistence.IdentityDocuments}, 
                { XPathes.ProofOfAddressDocument, dataExistence => dataExistence.AddressDocuments},   
                { XPathes.ProofOfFundsDocument, dataExistence => dataExistence.ProofOfFundsDocuments},
                { XPathes.W9Form, dataExistence => dataExistence.W9Form},                             
            };
        private readonly CollectionStepValidator _collectionStepValidator = new();

        public CollectionStepsValidator()
        {
            RuleFor(user => user.CollectionSteps)
                .Must(BeOnlyOneOpenStep);

            RuleForEach(user => user.CollectionSteps)
                .SetValidator(_collectionStepValidator);

            // Select actual steps and check if state suits data existence
            RuleForEach(user => SelectActualSteps(user))
                .Must(MustCorrespondDataExistence)
                .OverridePropertyName("Actual collection step state");
            
            RuleFor(user => user.CollectionSteps.SelectMany(step => step.RelatedTasks))
                .Must((user, relatedTasks) =>
                {
                    var applicationTasks = user.Tasks.Where(task => task.ApplicationId == user.Application.Id).Select(task => task.Id);
                    return !relatedTasks.Except(applicationTasks).Any();
                })
                .WithErrorCode(ErrorCodes.StepsMustBeAttachedOnlyToApplicationTasks);
        }

        private bool BeOnlyOneOpenStep(UserConsistency user, IEnumerable<CollectionStep> steps, ValidationContext<UserConsistency> context)
        {
            var xPathesWithSeveralOpenSteps = steps.GroupBy(step => step.XPath)
                                                   .Where(grouping => grouping.Count(step => !step.IsCompleted) > 1)
                                                   .Select(grouping => grouping.Key)
                                                   .ToArray();

            if (xPathesWithSeveralOpenSteps.Length == 0)
                return true;

            var validationFailure = new ValidationFailure("CollectionSteps", "More than one open collection step with same Xpath")
                {
                    ErrorCode = ErrorCodes.SeveralStepsInOpenStateWithSameXPath,
                    CustomState = string.Join(",", xPathesWithSeveralOpenSteps)
                };
            context.AddFailure(validationFailure);
            
            return true;
        }

        private bool MustCorrespondDataExistence(UserConsistency user, 
                                                 CollectionStep step, 
                                                 ValidationContext<UserConsistency> context)
        {
            if (!DataExistenceMapping.TryGetValue(step.XPath, out var getExistence))
                return true;
            
            var isDataExists = getExistence(context.InstanceToValidate.ProfileDataExistence);
            var state = step.State;
            
            if (state == CollectionStepState.Requested && isDataExists)
            {
                context.AddFailure(new ValidationFailure($"{nameof(step.State)}", $"Step requested {step.XPath}, when data already exists")
                {
                    ErrorCode = ErrorCodes.StepRequestedWhenDataExists,
                    CustomState = step,
                    // Actually it's a normal situation when step was re-requested from admin panel
                    // Later can be added logic how to checks if it is admin requests or resubmit request
                    Severity = Severity.Warning,
                });
            }
            
            if (state != CollectionStepState.Requested && !isDataExists)
            {
                context.AddFailure(new ValidationFailure($"{nameof(step.State)}", "")
                {
                    ErrorCode = ErrorCodes.StepInNotRequestedStateWhenDataAbsent,
                    CustomState = step
                });
            }

            return true;
        }

        private static IEnumerable<CollectionStep> SelectActualSteps(UserConsistency user) =>
            user.CollectionSteps.GroupBy(step => step.XPath)
                .Select(steps => steps.OrderBy(step => step.State)
                                      .ThenByDescending(step => step.CreatedAt).First());
    }
}