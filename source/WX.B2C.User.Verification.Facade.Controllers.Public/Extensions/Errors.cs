using System;
using Microsoft.AspNetCore.Mvc;
using Optional;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Facade.Controllers.Public.Responses;
using static WX.B2C.User.Verification.Facade.Controllers.Public.Responses.ErrorResponse;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Extensions
{
    public static class Errors
    {
        public static NotFoundObjectResult ProfileNotFound(Guid userId) =>
            new(Create(ErrorDetails.ProfileNotFound(userId)));

        public static ObjectResult ResidenceAddressNotFound(Guid userId) =>
            new(Create(ErrorDetails.ResidenceAddressNotFound(userId))) { StatusCode = 425 };

        public static NotFoundObjectResult ValidationRulesNotFound() =>
            new(Create(ErrorDetails.ValidationRulesNotFound()));

        public static NotFoundObjectResult ValidationRulesNotFound(params ActionType[] actionTypes) =>
            new(Create(ErrorDetails.ValidationRulesNotFound(actionTypes)));

        public static UnprocessableEntityObjectResult ApplicationNotAvailable(string verificationContextCountry,
                                                                              string verificationContextState) =>
            new(Create(ErrorDetails.ApplicationNotAvailable(verificationContextCountry, verificationContextState)));

        public static ConflictObjectResult ApplicationAlreadyCreated(Guid userId, Guid applicationId) =>
            new(Create(ErrorDetails.ApplicationAlreadyCreated(userId, applicationId)));

        public static UnprocessableEntityObjectResult ValidationError(string error) =>
            new(Create(ErrorDetails.CreateValidationError(error)));
    }
}