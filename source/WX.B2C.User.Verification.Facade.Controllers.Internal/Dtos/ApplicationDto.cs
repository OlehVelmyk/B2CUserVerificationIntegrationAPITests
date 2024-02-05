using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos
{
    public class ApplicationDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public ProductType ProductType { get; set; }

        public ApplicationState State { get; set; }

        [NotRequired]
        public ApplicationState? PreviousState { get; set; }

        public string[] DecisionReasons { get; set; }

        public DateTime CreatedAt { get; set; }

        [NotRequired]
        public DateTime? FirstApprovedDate { get; set; }

        [NotRequired]
        public DateTime? LastApprovedDate { get; set; }
    }
}
