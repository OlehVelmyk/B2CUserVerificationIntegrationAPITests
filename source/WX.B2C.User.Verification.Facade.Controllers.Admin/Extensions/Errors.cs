using System;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;
using static WX.B2C.User.Verification.Facade.Controllers.Admin.Responses.ErrorResponse;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Extensions
{
    public static class Errors
    {
        public static NotFoundObjectResult ApplicationNotFound(Guid userId, ProductType productType) =>
            new (Create(ErrorDetails.ApplicationNotFound(userId, productType)));

        public static NotFoundObjectResult ApplicationNotFound(Guid userId, Guid applicationId) =>
            new (Create(ErrorDetails.ApplicationNotFound(userId, applicationId)));
        
        public static NotFoundObjectResult CheckNotFound() =>
            new (Create(ErrorDetails.CheckNotFound()));   
        
        public static NotFoundObjectResult CheckVariantNotFound() =>
            new (Create(ErrorDetails.CheckVariantNotFound()));
        
        public static NotFoundObjectResult CollectionStepNotFound() =>
            new (Create(ErrorDetails.CollectionStepNotFound()));
        
        public static NotFoundObjectResult TaskNotFound() =>
            new (Create(ErrorDetails.TaskNotFound()));
        
        public static NotFoundObjectResult FileNotFound() =>
            new (Create(ErrorDetails.FileNotFound()));

        public static ConflictObjectResult FileUploadedToOtherProvider(ExternalFileProviderType provider) =>
            new(Create(ErrorDetails.FileUploadedToOtherProvider(provider)));

        public static NotFoundObjectResult DocumentNotFound() =>
            new (Create(ErrorDetails.DocumentNotFound()));

        public static NotFoundObjectResult ProfileNotFound() =>
            new (Create(ErrorDetails.ProfileNotFound()));

        public static NotFoundObjectResult NoteNotFound() =>
            new(Create(ErrorDetails.NoteNotFound()));

        public static ConflictObjectResult CollectionStepReviewNotNeeded() =>
            new (Create(ErrorDetails.CollectionStepReviewNotNeeded()));

        public static ConflictObjectResult CollectionStepInvalidState(CollectionStepState state) =>
            new(Create(ErrorDetails.CollectionStepInvalidState(state)));

        public static UnprocessableEntityObjectResult ValidationError(string error) =>
            new(Create(ErrorDetails.CreateValidationError(error)));
    }
}
