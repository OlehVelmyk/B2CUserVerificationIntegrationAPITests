using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class ApplicationDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid PolicyId { get; set; }

        public ProductType ProductType { get; set; }

        public ApplicationState State { get; set; }

        public ApplicationState? PreviousState { get; set; }

        public string[] DecisionReasons { get; set; }

        public ApplicationTaskDto[] Tasks { get; set; }

        public bool IsAutomating { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
