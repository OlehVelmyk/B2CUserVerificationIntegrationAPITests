using System;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static class ArbitraryExtensions
    {
        public static Gen<UserInfo> OverridePolicy(this Gen<UserInfo> gen, Gen<VerificationPolicy> newValueGen)
        {
            if (newValueGen is null) 
                throw new ArgumentNullException(nameof(newValueGen));

            return newValueGen.SelectMany(policy => gen.OverridePolicy(policy));
        }

        public static Gen<UserInfo> OverridePolicy(this Gen<UserInfo> gen, VerificationPolicy policy) =>
            from seed in Arb.Generate<Seed>()
            from nationality in CountryCodeGenerators.Supported(policy)
            from address in AddressGenerators.Address(seed, nationality)
            from userInfo in gen.Override(policy, ui => ui.Policy)
                                .Override(nationality, ui => ui.Nationality)
                                .Override(address, ui => ui.Address)
            select userInfo;
    }
}
