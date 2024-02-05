using FsCheck;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal static class StateGenerators
    {
        private static readonly string[] UsStates =
        {
            "SC",
            "NM",
            "OK",
            "GA",
            "RI",
            "WA",
            "MA",
            "KS",
            "WY",
            "CT",
            "AL",
            "MI",
            "UT",
            "NC",
            // "HI",
            "KY",
            "MD",
            "FL",
            "MT",
            "AK",
            "VT",
            "ID",
            "ME",
            "CA",
            "NJ",
            "MN",
            "IA",
            "WV",
            "AZ",
            "OR",
            "CO",
            "IN",
            "OH",
            "PA",
            "NE",
            "TN",
            "MO",
            "ND",
            // "NY",
            "VA",
            "NV",
            "IL",
            "WI",
            "DE",
            "TX",
            "LA",
            "AR",
            "MS",
            "NH",
            "DC",
            "SD"
        };

        private static readonly string[] UnsupportedUsStates =
        {
            "HI",
            "NY"
        };

        public static Gen<string> Us() => Gen.Elements(UsStates);

        public static Gen<string> UnsupportedUs() => Gen.Elements(UnsupportedUsStates);
    }
}
