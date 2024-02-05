using System;
using System.Linq;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class ApplicationStateChanged : DomainEvent
    {
        public Guid UserId { get; private set; }

        public Guid ApplicationId { get; private set; }

        public ApplicationState NewState { get; private set; }

        public ApplicationState PreviousState { get; private set; }

        public string[] DecisionReasons { get; private set; }

        public Initiation Initiation { get; private set; }

        public static ApplicationStateChanged Create(
            Application application,
            ApplicationState previousState,
            Initiation initiation)
        {
            return new()
            {
                UserId = application.UserId,
                ApplicationId = application.Id,
                NewState = application.State,
                PreviousState = previousState,
                DecisionReasons = application.DecisionReasons.ToArray(),
                Initiation = initiation
            };
        }
    }
}
