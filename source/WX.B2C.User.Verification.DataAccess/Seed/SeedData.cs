using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.DataAccess.Seed.Models;
using WX.B2C.User.Verification.DataAccess.Seed.Validators;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.DataAccess.Seed
{
    internal static class SeedData
    {
        #region Seed data

        private static readonly Lazy<CheckVariant[]> CheckVariantsData =
            GetLazyData<CheckVariant>(SeedConstants.CheckVariantsFileName);

        private static readonly Lazy<VerificationPolicy[]> VerificationPoliciesData =
            GetLazyData<VerificationPolicy>(SeedConstants.VerificationPoliciesFileNames,
                                            policy => policy.DefineCheckVariants(CheckVariantsData.Value));

        private static readonly Lazy<MonitoringPolicy[]> MonitoringPoliciesData =
            GetLazyData<MonitoringPolicy>(SeedConstants.MonitoringPoliciesFileNames);

        private static readonly Lazy<ValidationRule[]> ValidationRulesData =
            GetLazyData<ValidationRule>(SeedConstants.ValidationRulesFileName);

        private static readonly Lazy<ValidationPolicy[]> ValidationPoliciesData =
            GetLazyData<ValidationPolicy>(SeedConstants.ValidationPoliciesFileName);

        #endregion

        public static IReadOnlyCollection<Entities.Policy.VerificationPolicy> VerificationPolicies =>
            VerificationPoliciesData.Value.ToVerificationPolicy();
        public static IReadOnlyCollection<Entities.Policy.MonitoringPolicy> MonitoringPolicies =>
            MonitoringPoliciesData.Value.ToMonitoringPolicy();

        public static IReadOnlyCollection<Entities.Policy.TaskVariant> Tasks =>
            VerificationPoliciesData.Value.ToTasks().
            Concat(MonitoringPoliciesData.Value.ToTasks()).
            ToArray();

        public static IReadOnlyCollection<Entities.Policy.PolicyCheckVariant> ChecksVariants =>
            CheckVariantsData.Value.ToCheckVariantsEntities();

        public static IReadOnlyCollection<Entities.Policy.ValidationRule> ValidationRules =>
            ValidationRulesData.Value.ToValidationRuleEntities();

        public static IReadOnlyCollection<Entities.Policy.ValidationPolicy> ValidationPolicies =>
            ValidationPoliciesData.Value.ToValidationPolicyEntities();

        public static IReadOnlyCollection<Entities.Policy.PolicyValidationRule> PolicyValidationRules =>
            ValidationPoliciesData.Value.ToPolicyValidationRulesEntities();

        public static IReadOnlyCollection<Entities.Policy.TaskCheckVariant> TaskCheckVariants =>
            VerificationPoliciesData.Value.ToTaskCheckVariants();

        public static IReadOnlyCollection<Entities.Policy.TriggerVariant> Triggers =>
            VerificationPoliciesData.Value.ToTriggersEntities().
            Concat(MonitoringPoliciesData.Value.ToTriggersEntities()).
            ToArray();

        public static IReadOnlyCollection<Entities.Policy.PolicyTask> PolicyTasks =>
            VerificationPoliciesData.Value.ToPolicyTasks();
        
        private static Lazy<T[]> GetLazyData<T>(string[] files, Action<T> populateDataForValidation = null)
        {
            return new(() =>
            {
                var seedModels = files.SelectMany(SeedProvider.GetEntities<T>).ToArray();
                
                if (populateDataForValidation != null)
                    seedModels.Foreach(populateDataForValidation);

                return seedModels.ToArray().Validate();
            });
        }

        private static Lazy<T[]> GetLazyData<T>(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException(nameof(file));

            return new Lazy<T[]>(SeedProvider.GetEntities<T>(file).Validate);
        }
    }
}
