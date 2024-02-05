using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class Application : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid PolicyId { get; set; }

        public ProductType ProductType { get; set; }

        public ApplicationState State { get; set; }

        public ApplicationState? PreviousState { get; set; }

        public HashSet<string> DecisionReasons { get; set; }

        public HashSet<ApplicationTask> RequiredTasks { get; set; }

        public bool IsAutomating { get; set; }
    }
}