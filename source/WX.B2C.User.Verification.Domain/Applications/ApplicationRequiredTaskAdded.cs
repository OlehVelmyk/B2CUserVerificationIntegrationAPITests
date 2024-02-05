using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class ApplicationRequiredTaskAdded : DomainEvent
    {
        public Guid ApplicationId { get; private set; }

        public Guid UserId { get; private set; }

        public Guid TaskId { get; private set; }

        public Initiation Initiation { get; private set; }

        public static ApplicationRequiredTaskAdded Create(
            Models.Application application,
            ApplicationTask newRequiredTask,
            Initiation initiation)
        {
            return new()
            {
                ApplicationId = application.Id,
                UserId = application.UserId,
                TaskId = newRequiredTask.Id,
                Initiation = initiation
            };
        }
    }
}