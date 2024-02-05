using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    public class VerificationTaskNoRelationsSpecimen : VerificationTaskSpecimen
    {
        public VerificationTaskNoRelationsSpecimen()
        { }

        public VerificationTaskNoRelationsSpecimen(VerificationTaskSpecimen specimen)
        {
            Id = specimen.Id;
            UserId = specimen.UserId;
            Type = specimen.Type;
            VariantId = specimen.VariantId;
            State = specimen.State;
            Result = specimen.Result;
            ExpirationDetails = specimen.ExpirationDetails;
            CreationDate = specimen.CreationDate;
        }
    }

    public class VerificationTaskSpecimen
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public TaskType Type { get; set; }

        public Guid VariantId { get; set; }

        public TaskState State { get; set; }

        public TaskResult? Result { get; set; }

        public TaskExpirationDetails ExpirationDetails { get; set; }

        public HashSet<TaskCheck> PerformedChecks { get; set; }

        public HashSet<TaskCollectionStep> CollectionSteps { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
