using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;

namespace WX.B2C.User.Verification.DataAccess.Seed
{
    internal static class SeedToEntityConverter
    {
        public static VerificationPolicy[] ToVerificationPolicy(this Models.VerificationPolicy[] policies) 
            => policies.Select(ToEntity).ToArray();

        public static MonitoringPolicy[] ToMonitoringPolicy(this Models.MonitoringPolicy[] policies) 
            => policies.Select(ToEntity).ToArray();

        public static PolicyCheckVariant[] ToCheckVariantsEntities(this Models.CheckVariant[] policies) 
            => policies.Select(ToEntity).ToArray();

        public static ValidationRule[] ToValidationRuleEntities(this Models.ValidationRule[] rules) 
            => rules.Select(ToEntity).ToArray();

        public static ValidationPolicy[] ToValidationPolicyEntities(this Models.ValidationPolicy[] policies) 
            => policies.Select(ToEntity).ToArray();

        public static PolicyValidationRule[] ToPolicyValidationRulesEntities(this Models.ValidationPolicy[] policies) 
            => policies.SelectMany(ToEntities).ToArray();

        public static TaskCheckVariant[] ToTaskCheckVariants(this Models.VerificationPolicy[] policies) 
            => policies.SelectMany(ToTaskCheckVariantsEntities).ToArray();

        public static IEnumerable<TriggerVariant> ToTriggersEntities(this Models.VerificationPolicy[] policies) 
            => policies.SelectMany(ToTriggersEntities);

        public static IEnumerable<TriggerVariant> ToTriggersEntities(this Models.MonitoringPolicy[] policies) 
            => policies.SelectMany(ToTriggersEntities);

        public static PolicyTask[] ToPolicyTasks(this Models.VerificationPolicy[] policies) 
            => policies.SelectMany(ToPolicyTasks).ToArray();

        private static IEnumerable<PolicyValidationRule> ToEntities(Models.ValidationPolicy policy)
        {
            return policy.Rules.Select(ruleId => new PolicyValidationRule()
            {
                ValidationPolicyId = policy.Id,
                ValidationRuleId = ruleId
            });
        }

        private static ValidationPolicy ToEntity(Models.ValidationPolicy policy)
        {
            return new()
            {
                Id = policy.Id,
                Region = policy.Region,
                RegionType = (RegionType) policy.RegionType,
            };
        }

        private static ValidationRule ToEntity(Models.ValidationRule rule)
        {
            return new()
            {
                Id = rule.Id,
                RuleSubject = rule.RuleSubject,
                RuleType = rule.RuleType,
                Validation = JsonConvert.SerializeObject(rule.Validations),
            };
        }

        public static List<TaskVariant> ToTasks(this Models.VerificationPolicy[] policies)
        {
            var entities = new List<TaskVariant>();
            foreach (var verificationPolicy in policies)
            {
                entities.AddRange(verificationPolicy.Tasks.Select(task => task.ToEntity()));
                entities.AddRange(verificationPolicy.Templates?.Select(task => task.ToEntity()) ?? Array.Empty<TaskVariant>());
            }
            return entities;
        }

        public static List<TaskVariant> ToTasks(this Models.MonitoringPolicy[] policies)
        {
            var entities = new List<TaskVariant>();
            foreach (var verificationPolicy in policies)
            {
                entities.AddRange(verificationPolicy.Templates?.Select(task => task.ToEntity()) ?? Array.Empty<TaskVariant>());
            }
            return entities;
        }

        private static VerificationPolicy ToEntity(this Models.VerificationPolicy policy)
        {
            return new()
            {
                Id = policy.Id,
                Name = policy.Name,
                Region = policy.Region,
                RegionType = policy.RegionType,
                RejectionPolicy = policy.RejectionPolicy,
            };
        }
        private static MonitoringPolicy ToEntity(this Models.MonitoringPolicy policy)
        {
            return new()
            {
                Id = policy.Id,
                Name = policy.Name,
                Region = policy.Region,
                RegionType = policy.RegionType,
            };
        }
        
        private static TaskCheckVariant[] ToTaskCheckVariantsEntities(this Models.VerificationPolicy policy)
        {
            var entities = new List<TaskCheckVariant>();
            foreach (var task in policy.Tasks)
            {
                if (task.ChecksVariants == null)
                    continue;
                
                entities.AddRange(task.ChecksVariants.Select(checkVariant => new TaskCheckVariant
                {
                    TaskId = task.Id,
                    CheckVariantId = checkVariant
                }));
            }
            return entities.ToArray();
        }

        private static TriggerVariant[] ToTriggersEntities(this Models.VerificationPolicy policy)
        {
            if (policy.Triggers == null)
                return Array.Empty<TriggerVariant>();

            return policy.Triggers.Select(trigger => Map(trigger, policy.Id)).ToArray();
        }        
        
        private static TriggerVariant[] ToTriggersEntities(this Models.MonitoringPolicy policy)
        {
            if (policy.Triggers == null)
                return Array.Empty<TriggerVariant>();

            return policy.Triggers.Select(trigger => Map(trigger, policy.Id)).ToArray();
        }

        private static TriggerVariant Map(Models.Trigger trigger, Guid policyId)
        {
            if (trigger == null)
                throw new ArgumentNullException(nameof(trigger));

            return new()
            {
                Id = trigger.Id,
                PolicyId = policyId,
                Name = trigger.Name,
                Iterative = trigger.Iterative,
                Schedule = trigger.Schedule == null ? null : Map(trigger.Schedule),
                Preconditions = trigger.Preconditions,
                Conditions = trigger.Conditions,
                Commands = trigger.Commands.Select(Map).ToArray()
            };
        }

        private static Command Map(Models.Command command)
        {
            var config = JsonConvert.SerializeObject(command.Config);
            return new Command
            {
                Type = command.Type,
                Config = config
            };
        }

        private static Schedule Map(Models.Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            var value = JsonConvert.SerializeObject(schedule.Value);
            return new Schedule
            {
                Type = schedule.Type,
                Offset = schedule.Offset,
                Value = value
            };
        }

        private static PolicyTask[] ToPolicyTasks(this Models.VerificationPolicy policy)
        {
            return policy.Tasks.Select(task => new PolicyTask
            {
                TaskVariantId = task.Id,
                PolicyId = policy.Id,
            }).ToArray();
        }

        private static PolicyCheckVariant ToEntity(this Models.CheckVariant policy)
        {
            return new()
            {
                Id = policy.Id,
                Name = policy.Name,
                Type = policy.Type,
                Provider = policy.Provider,
                Config = policy.Config == null ? null : JsonConvert.SerializeObject(policy.Config),
                FailResultType = (CheckResultType?) policy.FailResult?.Type,
                FailResult = policy.FailResult?.Result == null ? null : JsonConvert.SerializeObject(policy.FailResult.Result),
                FailResultCondition = policy.FailResult?.Condition == null ? null : JsonConvert.SerializeObject(policy.FailResult.Condition),
                RunPolicy = policy.RunPolicy != null ? Map(policy.RunPolicy) : null,
            };
        }

        private static CheckRunPolicy Map(Models.CheckRunPolicy runPolicy)
        {
            return new CheckRunPolicy { MaxAttempts = runPolicy.MaxAttempts };
        }

        private static TaskVariant ToEntity(this Models.Task task)
        {
            return new()
            {
                Id = task.Id,
                Name = task.Name,
                Type = task.Type,
                Priority = task.Priority,
                CollectionSteps = task.CollectionSteps,
                AutoCompletePolicy = task.AutoCompletePolicy,
            };
        }
    }
}