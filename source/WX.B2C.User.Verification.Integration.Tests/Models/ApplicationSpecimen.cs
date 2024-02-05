using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    internal class ApplicationNoRelationsSpecimen : ApplicationSpecimen
    {
        public ApplicationNoRelationsSpecimen()
        { }

        public ApplicationNoRelationsSpecimen(ApplicationSpecimen specimen)
        {
            Id = specimen.Id;
            UserId = specimen.UserId;
            PolicyId = specimen.PolicyId;
            ProductType = specimen.ProductType;
            State = specimen.State;
            PreviousState = specimen.PreviousState;
            DecisionReasons = specimen.DecisionReasons;
            RequiredTasks = Array.Empty<VerificationTaskSpecimen>();
        }
    }

    internal class ApplicationSpecimen
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid PolicyId { get; set; }

        public ProductType ProductType { get; set; }

        public ApplicationState State { get; set; }

        public ApplicationState? PreviousState { get; set; }

        public HashSet<string> DecisionReasons { get; set; }

        public VerificationTaskSpecimen[] RequiredTasks { get; set; }
    }
}
