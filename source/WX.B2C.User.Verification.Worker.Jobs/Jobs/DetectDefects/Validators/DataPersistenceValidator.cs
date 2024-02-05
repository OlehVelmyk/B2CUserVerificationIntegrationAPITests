using System.Linq;
using FluentValidation;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators
{
    internal class DataPersistenceValidator : AbstractValidator<UserConsistency>
    {
        public DataPersistenceValidator()
        {
            RuleFor(user => user.ProfileDataExistence.IpAddress).Equal(true).WithErrorCode(ErrorCodes.AbsentIpAddress);
            RuleFor(user => user.ProfileDataExistence.FullName).Equal(true).WithErrorCode(ErrorCodes.AbsentFullName);
            RuleFor(user => user.ProfileDataExistence.DateOfBirth).Equal(true).WithErrorCode(ErrorCodes.AbsentBirthdate);
            RuleFor(user => user.ProfileDataExistence.TaxResidence).Equal(true).WithErrorCode(ErrorCodes.AbsentTaxResidence);

            RuleFor(user => user.ProfileDataExistence.IdDocumentNumberType)
                .Equal(true)
                .When(user => user.ProfileDataExistence.IdDocumentNumber)
                .WithErrorCode(ErrorCodes.IdDocumentTypeAbsentWhenNumberExists);

            RuleFor(user => user.ProfileDataExistence.IdDocumentNumber)
                .Equal(true)
                .When(user => user.Tasks.FirstOrDefault(task => task.Type == TaskType.Identity)?.State == TaskState.Completed)
                .WithErrorCode(ErrorCodes.AbsentIdDocNumberWhenIdentityTaskCompleted);

            RuleFor(user => user.ProfileDataExistence.IdentityDocuments)
                .Equal(true)
                .When(user => user.Tasks.FirstOrDefault(task => task.Type == TaskType.Identity)?.State == TaskState.Completed)
                .WithErrorCode(ErrorCodes.AbsentIdentityDocumentsWhenIdentityTaskCompleted);

            RuleFor(user => user.ProfileDataExistence.ProofOfFundsDocuments)
                .Equal(true)
                .When(user => user.Tasks.FirstOrDefault(task => task.Type == TaskType.ProofOfFunds)?.State == TaskState.Completed)
                .WithErrorCode(ErrorCodes.AbsentPofDocumentsWhenPofTaskCompleted);

            RuleFor(user => user.ProfileDataExistence.Tin)
                .Equal(true)
                .When(user => user.Region is Region.USA)
                .WithErrorCode(ErrorCodes.AbsentTinWhenUSA);
        }
    }
}