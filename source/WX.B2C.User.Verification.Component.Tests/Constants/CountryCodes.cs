using System.Linq;

namespace WX.B2C.User.Verification.Component.Tests.Constants
{
    internal static class CountryCodes
    {
        public const string Gb = "GB";
        public const string Us = "US";
        public const string Ru = "RU";
        public const string Ph = "PH";

        public static readonly string[] Eea =
        {
            "AD",
            "AT",
            "BE",
            "BG",
            "HR",
            "CY",
            "CZ",
            "DK",
            "EE",
            "FI",
            "FR",
            "DE",
            // "GB",
            "GI",
            "GR",
            "HU",
            "IS",
            "IE",
            "IT",
            "LV",
            "LI",
            "LT",
            "LU",
            "MT",
            "MC",
            "ME",
            "NL",
            "NO",
            "PL",
            "PT",
            "RO",
            "SK",
            "SI",
            "ES",
            "SE",
            "CH",
        };

        public static readonly string[] Apac =
        {
             "AU",
             "HK",
             "KR",
             "NZ",
             // "PH",
             "SG",
             "TW",
             "TH",
             "VN"
        };

        public static readonly string[] RoW =
        {
            "AX",
            "AL",
            "AG",
            "AR",
            "AM",
            "AW",
            "BH",
            "BB",
            "BZ",
            "BJ",
            "BM",
            "BT",
            "BF",
            "KH",
            "CM",
            "KY",
            "CL",
            "CO",
            "CK",
            "CR",
            "DJ",
            "DM",
            "DO",
            "EC",
            "SV",
            "FO",
            "FJ",
            "GA",
            "GE",
            "GD",
            "GT",
            "GG",
            "HN",
            "IM",
            "IL",
            "JM",
            "JE",
            "KZ",
            "KE",
            "KW",
            "KG",
            "LS",
            "MW",
            "MY",
            "ML",
            "MR",
            "MX",
            "MD",
            "MS",
            "NA",
            "NI",
            "NE",
            "NG",
            "NF",
            "OM",
            "PA",
            "PY",
            "PE",
            "PR",
            "QA",
            "KN",
            "LC",
            "VC",
            "WS",
            "TJ",
            "TZ",
            "TG",
            "TM",
            "TC",
            "TV",
            "UA",
            "AE",
            // "US",
            "UZ",
            "VG",
            "ZM",
            // "RU"
        };

        public static readonly string[] Global =
        {
            "BA",
            "BN",
            "BR",
            "CA",
            "DZ",
            "ID",
            "IN",
            "MA",
            "NP",
            "PK",
            "TR",
             "XK",
            "ZA"
        };

        public static readonly string[] Supported =
            Eea.Concat(Apac)
               .Concat(RoW)
               .Concat(Global)
               .Concat(new[] { Gb, Us, Ru, Ph })
               .ToArray();

        public static readonly string[] Unsupported = { "CU" };
    }
}
