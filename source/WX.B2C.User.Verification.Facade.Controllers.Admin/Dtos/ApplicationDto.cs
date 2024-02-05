using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Enums;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class ApplicationDto
    {
        public Guid Id { get; set; }

        public ApplicationState State { get; set; }

        public ApplicationAction[] AllowedActions { get; set; }

        public string[] DecisionReasons { get; set; }

        public ApplicationTaskDto[] RequiredTasks { get; set; }

        public bool IsAutomating { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class ApplicationTaskDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Priority { get; set; }

        public TaskState State { get; set; }

        [NotRequired]
        public TaskResult? Result { get; set; }
    }
}