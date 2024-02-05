using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks
{
    /// <summary>
    /// Provide dynamic tasks (tasks which can be added to application in the process of verification)
    /// </summary>
    internal class HardcodedDynamicTasksProvider : IDynamicTasksProvider
    {
        private static TaskVariant[] EeaTasks =
        {
            new()
            {
                Type = TaskType.ProofOfFunds,
                Id = Guid.Parse("9E202236-42B4-4105-A6A4-1356A82911A2")
            },
            new()
            {
                Type = TaskType.FinancialCondition,
                Id = Guid.Parse("41F23381-F8BB-42C7-8B58-C344A4AD011E")
            }
        };

        private static TaskVariant[] GbTasks =
        {
            new()
            {
                Type = TaskType.FinancialCondition,
                Id = Guid.Parse("CF9F0CA8-6535-4F4A-B2D3-71AB075EC841")
            },
            new()
            {
                Type = TaskType.ProofOfFunds,
                Id = Guid.Parse("C1B826B7-6E0A-4AB3-813D-4393E0C0E095")
            }
        };

        private static TaskVariant[] UsaTasks =
        {
            new()
            {
                Type = TaskType.UserRiskScreening,
                Id = Guid.Parse("C2D32093-AD0F-45AD-9377-4DD12550A221")
            }
        };

        private static TaskVariant[] RoWTasks =
        {
            new()
            {
                Type = TaskType.ProofOfFunds,
                Id = Guid.Parse("1673241E-BC0A-4007-A353-A2C39880BBEF")
            }
        };

        private static TaskVariant[] ApacTasks = Array.Empty<TaskVariant>();
        private static TaskVariant[] GlobalTasks = Array.Empty<TaskVariant>();
        private static TaskVariant[] RuTasks = Array.Empty<TaskVariant>();

        private static Dictionary<Guid, TaskVariant[]> PolicyDynamicTasks = new()
        {
            { Guid.Parse("0EAAE368-8ACB-410B-8EC0-3AE404F49D5E"), EeaTasks },
            { Guid.Parse("DC658B4F-A0EB-4C20-B296-E0D57E8DA6DB"), GbTasks },
            { Guid.Parse("37C6AD01-067C-4B80-976D-30A568E7B0CD"), ApacTasks },
            { Guid.Parse("4B6271BD-FDE5-40F7-8701-29AA66865568"), UsaTasks },
            { Guid.Parse("D5B5997E-FFC1-495D-9E98-60CCBDD6F43B"), RoWTasks },
            { Guid.Parse("5DECE2A9-CDD3-4D0D-B1BC-8A164B745051"), GlobalTasks },
            { Guid.Parse("67A2B2C8-BEAB-4C3E-A772-19CE9380CB0E"), RuTasks },
        };

        public TaskVariant Find(Guid policyId, TaskType taskType)
        {
            var taskVariants = PolicyDynamicTasks[policyId];
            return taskVariants.FirstOrDefault(tv => tv.Type == taskType);
        }

        public TaskVariant[] Get(Guid policyId) =>
            PolicyDynamicTasks[policyId];
    }
}
