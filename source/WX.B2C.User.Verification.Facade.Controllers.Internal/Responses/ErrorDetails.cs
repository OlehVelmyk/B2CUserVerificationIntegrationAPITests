using System;
using System.ComponentModel.DataAnnotations;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Responses
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

        public static ErrorDetails CreateValidationError(string errorReason) =>
            new()
            {
                ErrorCode = Constants.ErrorCodes.ValidationError,
                Description = Constants.ErrorMessages.ValidationError,
                ErrorReason = errorReason
            };

        public static ErrorDetails ProfileNotFound(Guid userId) =>
            Create(Constants.ErrorCodes.ProfileNotFoundError,
                   $"Not found profile for user {userId}");

        public static ErrorDetails ApplicationNotFound(Guid userId, ProductType productType) =>
            Create(Constants.ErrorCodes.ApplicationNotFoundError,
                   $"Not found application for user {userId} with type {productType}");

        public static ErrorDetails ApplicationNotFound(Guid userId, Guid applicationId) =>
            Create(Constants.ErrorCodes.ApplicationNotFoundError,
                   $"Not found application for user {userId} with id {applicationId}");

        public static ErrorDetails DocumentNotFound(Guid userId, DocumentCategory category) =>
            Create(Constants.ErrorCodes.DocumentNotFoundError,
                   $"Not found document for user {userId} with category {category}");
    }
}