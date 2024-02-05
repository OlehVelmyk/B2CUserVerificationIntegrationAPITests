using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;

namespace WX.B2C.User.Verification.Component.Tests.Providers
{
    internal interface ITriggerProvider
    {
        public Guid[] GetOnbording(VerificationPolicy policy);

        public Guid[] GetMonitoring(VerificationPolicy policy);
    }

    internal class HardCodedTriggerProvider : ITriggerProvider
    {
        private static readonly IEnumerable<Guid> GbOnbordingTriggers = new[]
        {
            new Guid("222E9D58-CACB-4094-9399-2AA50AA2D766"),
            new Guid("AF4321CB-E246-479A-9F13-84AC8942C078"),
            new Guid("D064BF48-8B8C-44C7-851C-59A69E78C37A")
        };
        private static readonly IEnumerable<Guid> EaaOnbordingTriggers = new[]
        {
            new Guid("15CF6520-6990-4F6D-A6A2-E4416DD55906"),
            new Guid("86107529-FE66-4817-8261-3AB8E860885E"),
            new Guid("90ABD5EC-84FF-4E9C-A3DB-4B5586116D3C")
        };
        private static readonly IEnumerable<Guid> UsOnbordingTriggers = new[]
        {
            new Guid("44FE4C45-7F6D-48A8-8690-E5D92C97F61B")
        };

        private static readonly IEnumerable<Guid> ApacMonitoringTriggers = new[]
        {
            new Guid("2B6467DB-BA15-42E5-9B3D-A4DF4FAE2475"),
            new Guid("E8671ADD-B3F4-4C61-ABB8-310B31AF9C2E"),
        };
        private static readonly IEnumerable<Guid> EaaMonitoringTriggers = new[]
        {
            new Guid("B62B049C-4B5C-40E5-8927-D83674E2A245"),
            new Guid("14FA6B41-8751-48FA-A862-EC7486A7B06C"),
            new Guid("25540161-4CCB-4C03-9458-62CD8DAD0BFA"),
            new Guid("9BD82EB1-4649-460A-98B3-491A2C79A509"),
            new Guid("E1FFB995-72CB-429E-99B2-DB4282DA6526"),
        };
        private static readonly IEnumerable<Guid> GbMonitoringTriggers = new[]
        {
            new Guid("64E483BC-20F6-4EEC-9F1A-2F2363CFC861"),
            new Guid("044529B5-D983-4F57-8DBA-16FEBF264052"),
            new Guid("F623D69C-F636-4DAC-923A-5BE2C3779D7B"),
            new Guid("6F762B87-112E-43C5-B311-C32CDF723D63"),
        };
        private static readonly IEnumerable<Guid> PhMonitoringTriggers = new[]
        {
            new Guid("139C0711-AE5D-42A5-9CE4-3DE2877D4D86"),
            new Guid("13FA5763-CD13-4C38-9586-81F6C80183CC"),
        };
        private static readonly IEnumerable<Guid> RowMonitoringTriggers = new[]
        {
            new Guid("651F41FE-29F9-4F2A-979D-F425377DB305"),
            new Guid("F7AEF22C-0638-4322-97FD-E5DA05918D1F"),
            new Guid("47EAA9B2-0FE0-41E0-A1E8-B502F48A9C23"),
            new Guid("C8C1ECF5-C784-41BB-8FAD-C30CFAC68918"),
            new Guid("82CC6921-9263-442A-B022-00A8D1D5A4BF"),
        };

        public Guid[] GetOnbording(VerificationPolicy policy) =>
            policy switch
            {
                VerificationPolicy.Gb          => GbOnbordingTriggers.ToArray(),
                VerificationPolicy.Us          => UsOnbordingTriggers.ToArray(),
                VerificationPolicy.Eaa         => EaaOnbordingTriggers.ToArray(),
                VerificationPolicy.Apac        => Array.Empty<Guid>(),
                VerificationPolicy.Ph          => Array.Empty<Guid>(),
                VerificationPolicy.Global      => Array.Empty<Guid>(),
                VerificationPolicy.Row         => Array.Empty<Guid>(),
                VerificationPolicy.Ru          => Array.Empty<Guid>(),
                VerificationPolicy.Unsupported => Array.Empty<Guid>(),
                _                              => throw new ArgumentOutOfRangeException(nameof(policy), policy, null)
            };

        public Guid[] GetMonitoring(VerificationPolicy policy) =>
            policy switch
            {
                VerificationPolicy.Gb          => GbMonitoringTriggers.ToArray(),
                VerificationPolicy.Eaa         => EaaMonitoringTriggers.ToArray(),
                VerificationPolicy.Row         => RowMonitoringTriggers.ToArray(),
                VerificationPolicy.Us          => RowMonitoringTriggers.ToArray(),
                VerificationPolicy.Ru          => RowMonitoringTriggers.ToArray(),
                VerificationPolicy.Apac        => ApacMonitoringTriggers.ToArray(),
                VerificationPolicy.Ph          => PhMonitoringTriggers.ToArray(),
                VerificationPolicy.Global      => Array.Empty<Guid>(),
                VerificationPolicy.Unsupported => Array.Empty<Guid>(),
                _                              => throw new ArgumentOutOfRangeException(nameof(policy), policy, null)
            };
    }
}
