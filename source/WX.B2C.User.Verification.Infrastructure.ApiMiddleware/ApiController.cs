using System;
using Microsoft.AspNetCore.Mvc;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware
{
    /// <summary>
    /// The main point to have this class is to show developer that using base controller functionality to
    /// return not successful response is prohibited.
    /// All not unsuccessful responses must have unified contract.
    /// </summary>
    public abstract class ApiController : ControllerBase
    {
        private const string UseOverloadErrorMessage = "Must be used rrors static methods to return unsuccessful response";
        private const string UseOverloadWithErrorResponse = "Use Errors static methods to return unsuccessful response";

        [Obsolete(UseOverloadWithErrorResponse, true)]
        public override NotFoundResult NotFound() =>
            throw new NotSupportedException(UseOverloadErrorMessage);

        [Obsolete(UseOverloadWithErrorResponse, true)]
        public override NotFoundObjectResult NotFound(object value) =>
            throw new NotSupportedException(UseOverloadErrorMessage);

        [Obsolete(UseOverloadWithErrorResponse, true)]
        public override BadRequestResult BadRequest() =>
            throw new NotSupportedException(UseOverloadErrorMessage);

        [Obsolete(UseOverloadWithErrorResponse, true)]
        public override BadRequestObjectResult BadRequest(object error) =>
            throw new NotSupportedException(UseOverloadErrorMessage);

        [Obsolete(UseOverloadWithErrorResponse, true)]
        public override UnprocessableEntityResult UnprocessableEntity() =>
            throw new NotSupportedException(UseOverloadErrorMessage);

        [Obsolete(UseOverloadWithErrorResponse, true)]
        public override UnprocessableEntityObjectResult UnprocessableEntity(object error) =>
            throw new NotSupportedException(UseOverloadErrorMessage);

        [Obsolete(UseOverloadWithErrorResponse, true)]
        public override ConflictResult Conflict() =>
            throw new NotSupportedException(UseOverloadErrorMessage);

        [Obsolete(UseOverloadWithErrorResponse, true)]
        public override ConflictObjectResult Conflict(object error) =>
            throw new NotSupportedException(UseOverloadErrorMessage);
    }
}