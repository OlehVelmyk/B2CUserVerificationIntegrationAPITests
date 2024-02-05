using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FsCheck;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WX.B2C.User.Verification.DataAccess.Seed;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Models;
using static WX.B2C.User.Verification.Integration.Tests.Constants;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators
{
    internal class PolicyTaskGenerator
    {
        private static PolicyTaskSpecimen[] AllSpecimens = CreateAllSpecimens();
        private static PolicyTaskSpecimen[] RuSpecimens = CreateRuSpecimens();
        private static UsPolicyTaskSpecimen[] UsSpecimens = CreateUsSpecimens();
        private static PolicyTaskSpecimen[] ApacSpecimens = CreateApacSpecimens();
        private static PolicyTaskSpecimen[] EeaSpecimens = CreateEeaSpecimens();
        private static PolicyTaskSpecimen[] RowSpecimens = CreateRowSpecimens();
        private static PolicyTaskSpecimen[] GlobalSpecimens = CreateGlobalSpecimens();
        private static GbPolicyTaskSpecimen[] GbSpecimens = CreateGbSpecimens();
        private static IdentityPolicyTaskSpecimen[] IdentitySpecimens = CreateIdentitySpecimens();

        public static Gen<PolicyTaskSpecimen> Any() =>
            Gen.Elements(AllSpecimens);

        public static Gen<GbPolicyTaskSpecimen> Gb() =>
            Gen.Elements(GbSpecimens);

        public static Gen<GbPoFPolicyTaskSpecimen> GbPoF() =>
            Gen.Constant(GbSpecimens.Single(s => s.TaskType == TaskType.ProofOfFunds))
               .Select(s => new GbPoFPolicyTaskSpecimen(s.PolicyId, s.TaskVariantId, s.TaskType));

        public static Gen<UsPolicyTaskSpecimen> Us() =>
            Gen.Elements(UsSpecimens);

        public static Gen<PolicyTasksSpecimen<UsPolicyTaskSpecimen>> UsAll() =>
            Gen.Constant(new PolicyTasksSpecimen<UsPolicyTaskSpecimen>(UsSpecimens));

        public static Gen<PolicyTasksSpecimen<IdentityPolicyTaskSpecimen>> IdentityAll() =>
            Gen.Constant(new PolicyTasksSpecimen<IdentityPolicyTaskSpecimen>(IdentitySpecimens));

        private static PolicyTaskSpecimen[] CreateAllSpecimens()
        {
            var staticTasks = SeedData.PolicyTasks
                                      .Join(SeedData.Tasks,
                                            pt => pt.TaskVariantId,
                                            tv => tv.Id,
                                            (pt, tv) => new PolicyTaskSpecimen(pt.PolicyId, pt.TaskVariantId, tv.Type));

            var dynamicTasksSourcePath = Path.Combine(Constants.RootPath, Arbitrary.SourcesFolder, Arbitrary.DynamicTasks);
            var json = File.ReadAllText(dynamicTasksSourcePath);
            var dynamicTasks = JsonConvert.DeserializeObject<PolicyTaskSpecimen[]>(json, new JsonSerializerSettings { Converters = { new StringEnumConverter() } });

            return staticTasks.Concat(dynamicTasks).ToArray();
        }

        private static IEnumerable<PolicyTaskSpecimen> All(TaskType taskType) =>
            AllSpecimens.Where(s => s.TaskType == taskType);

        private static IEnumerable<PolicyTaskSpecimen> All(Guid policyId) =>
            AllSpecimens.Where(s => s.PolicyId == policyId);

        private static IdentityPolicyTaskSpecimen[] CreateIdentitySpecimens() =>
            All(TaskType.Identity).Select(s => new IdentityPolicyTaskSpecimen(s.PolicyId, s.TaskVariantId, s.TaskType)).ToArray();

        private static PolicyTaskSpecimen[] CreateRuSpecimens() =>
            All(new Guid("67A2B2C8-BEAB-4C3E-A772-19CE9380CB0E")).ToArray();

        private static UsPolicyTaskSpecimen[] CreateUsSpecimens() =>
            All(new Guid("4B6271BD-FDE5-40F7-8701-29AA66865568")).Select(s => new UsPolicyTaskSpecimen(s.PolicyId, s.TaskVariantId, s.TaskType)).ToArray();

        private static PolicyTaskSpecimen[] CreateApacSpecimens() =>
            All(new Guid("37C6AD01-067C-4B80-976D-30A568E7B0CD")).ToArray();

        private static PolicyTaskSpecimen[] CreateEeaSpecimens() =>
            All(new Guid("0EAAE368-8ACB-410B-8EC0-3AE404F49D5E")).ToArray();

        private static PolicyTaskSpecimen[] CreateRowSpecimens() =>
            All(new Guid("D5B5997E-FFC1-495D-9E98-60CCBDD6F43B")).ToArray();

        private static PolicyTaskSpecimen[] CreateGlobalSpecimens() =>
            All(new Guid("5DECE2A9-CDD3-4D0D-B1BC-8A164B745051")).ToArray();

        private static GbPolicyTaskSpecimen[] CreateGbSpecimens() =>
            All(new Guid("DC658B4F-A0EB-4C20-B296-E0D57E8DA6DB")).Select(s => new GbPolicyTaskSpecimen(s.PolicyId, s.TaskVariantId, s.TaskType)).ToArray();
    }
}
