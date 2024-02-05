using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Optional;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using static WX.B2C.User.Verification.Facade.Controllers.Public.Constants.ErrorCodes; 

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Responses
{
    public class ErrorDetails
    {
        [Required]
        public string ErrorCode { get; set; }

        [Required]
        public string Description { get; set; }
        
        public Dictionary<string, object> Parameters { get; set; }
        
        public static ErrorDetails Create(string errorCode, string description) =>
            Create(errorCode, description, null);

        public static ErrorDetails Create(string errorCode, string description, Dictionary<string, object> parameters) =>
            new()
            {
                ErrorCode = errorCode,
                Description = description,
                Parameters = parameters
            };

        public static ErrorDetails CreateValidationError(string description) =>
            new()
            {
                ErrorCode = ValidationError,
                Description = description,
            };

        public static ErrorDetails ProfileNotFound(Guid userId) =>
            Create(ProfileNotFoundError, $"Profile for user {userId} is not found.");

        public static ErrorDetails ResidenceAddressNotFound(Guid userId) =>
            Create(UserAddressNotFoundError, $"User address within country is not defined yet for {userId}.");

        public static ErrorDetails ValidationRulesNotFound() =>
            Create(ValidationRulesNotFoundError, "Not found any available validation rules.");

        public static ErrorDetails ValidationRulesNotFound(params ActionType[] actionTypes) =>
            Create(ValidationRulesNotFoundError,
                   $"Not found any available validation rules for actions {string.Join(",", actionTypes)}.");

        public static ErrorDetails ApplicationNotAvailable(string country, string state) =>
            Create(ApplicationNotAvailableError,
                   $"Application is not available for {country}, {state}.");

        public static ErrorDetails ApplicationAlreadyCreated(Guid userId, Guid applicationId) =>
            Create(ApplicationAlreadyCreatedError,
                   $"Application {applicationId} already created for user {userId}.");
    }
}
