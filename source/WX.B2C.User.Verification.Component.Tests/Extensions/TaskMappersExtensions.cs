using FluentAssertions;
using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static class TaskMappersExtensions
    {
        public static ApplicationTaskDto Map(this TaskDto task) =>
            new ()
            {
                Id = task.Id,
                Name = task.Variant.Name,
                Result = task.Result,
                State = task.State,
                Priority = task.Priority
            };
    }
}
