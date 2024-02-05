using System;
using System.Linq;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class ApplicationRegistered : DomainEvent
    {
        public Guid ApplicationId { get; private set; }

        public Guid UserId { get; private set; }

        public Guid PolicyId { get; private set; }

        public ProductType ProductType { get; private set; }

        public Guid[] RequiredTasks { get; private set; }

        public Initiation Initiation { get; private set; }

        public static ApplicationRegistered Create(Application application, Initiation initiation) =>
            new()
            {
                ApplicationId = application.Id,
                UserId = application.UserId,
                PolicyId = application.PolicyId,
                ProductType = application.ProductType,
                RequiredTasks = application.RequiredTasks.Select(x => x.Id).ToArray(),
                Initiation = initiation
            };
    }
}
