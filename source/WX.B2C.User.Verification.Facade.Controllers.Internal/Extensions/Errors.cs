using System;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Responses;
using static WX.B2C.User.Verification.Facade.Controllers.Internal.Responses.ErrorResponse;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Extensions
{
    public static class Errors
    {
        public static NotFoundObjectResult ApplicationNotFound(Guid userId, ProductType productType) =>
            new (Create(ErrorDetails.ApplicationNotFound(userId, productType)));

        public static NotFoundObjectResult ApplicationNotFound(Guid userId, Guid applicationId) =>
            new(Create(ErrorDetails.ApplicationNotFound(userId, applicationId)));

        public static NotFoundObjectResult DocumentNotFound(Guid userId, DocumentCategory category) =>
            new (Create(ErrorDetails.DocumentNotFound(userId, category)));        
        
        public static NotFoundObjectResult ProfileNotFound(Guid userId) =>
            new (Create(ErrorDetails.ProfileNotFound(userId)));
    }
}