using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class ApplicationAutomated : DomainEvent
    {
        private ApplicationAutomated(Guid applicationId, Guid userId, Initiation initiation)
        {
            ApplicationId = applicationId;
            UserId = userId;
            Initiation = initiation;
        }

        public static ApplicationAutomated Create(Application application, Initiation initiation) =>
            new(application.Id, application.UserId, initiation);

        public Guid ApplicationId { get; }

        public Guid UserId { get; }

        public Initiation Initiation { get; }
    }
}