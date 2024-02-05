using System;
using System.ComponentModel.DataAnnotations;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Responses
{
    public class ErrorDetails
    {
        [Required]
        public string ErrorCode { get; set; }

        [Required]
        public string ErrorReason { get; set; }

        public (string property, string value)[] ErrorParameters { get; set; }

        [Required]
        public string Description { get; set; }

        public static ErrorDetails Create(string errorCode, string description, string errorReason) =>
            new()
            {
                ErrorCode = errorCode,
                ErrorReason = errorReason,
                Description = description
            };

        public static ErrorDetails Create(string errorCode, string errorReason) =>
            new()
            {
                ErrorCode = errorCode,
                ErrorReason = errorReason,
            };

        public static ErrorDetails CreateValidationError(string errorReason)
        {
            return new ErrorDetails
            {
                ErrorCode = Constants.ErrorCodes.ValidationError,
                Description = Constants.ErrorMessages.ValidationError,
                ErrorReason = errorReason
            };
        }

        public static ErrorDetails ProfileNotFound() =>
            Create(Constants.ErrorCodes.ProfileNotFoundError,
                   "Not found profile for user");

        public static ErrorDetails ApplicationNotFound(Guid userId, ProductType productType) =>
            Create(Constants.ErrorCodes.ApplicationNotFoundError,
                   $"Not found application for user {userId} with type {productType}");

        public static ErrorDetails ApplicationNotFound(Guid userId, Guid applicationId) =>
            Create(Constants.ErrorCodes.ApplicationNotFoundError,
                   $"Not found application for user {userId} with application {applicationId}");

        public static ErrorDetails CheckNotFound() =>
            Create(Constants.ErrorCodes.CheckNotFoundError, "Check not found");
        
        public static ErrorDetails CheckVariantNotFound() =>
            Create(Constants.ErrorCodes.CheckVariantNotFound, "Check variant not found");

        public static ErrorDetails CollectionStepNotFound() =>
            Create(Constants.ErrorCodes.CollectionStepNotFoundError, "Collection step not found");

        public static ErrorDetails FileNotFound() =>
            Create(Constants.ErrorCodes.FileNotFoundError, "File not found");
        
        public static ErrorDetails TaskNotFound() =>
            Create(Constants.ErrorCodes.TaskNotFoundError, "Task not found");

        public static ErrorDetails DocumentNotFound() =>
            Create(Constants.ErrorCodes.DocumentNotFoundError, "Document not found");

        public static ErrorDetails CollectionStepReviewNotNeeded() =>
            Create(Constants.ErrorCodes.CollectionStepReviewNotNeededError, "Collection step cannot be reviewed as it is not in review state");

        public static ErrorDetails NoteNotFound() =>
            Create(Constants.ErrorCodes.NoteNotFound, "Note not found");

        public static ErrorDetails CollectionStepInvalidState(CollectionStepState state) =>
            Create(Constants.ErrorCodes.CollectionStepInvalidState, $"Collection step state should be {state}");

        public static ErrorDetails FileUploadedToOtherProvider(ExternalFileProviderType provider) =>
            Create(Constants.ErrorCodes.FileUploadedToOtherProvider, $"File already uploaded to other provider: {provider}.");

    }
}
