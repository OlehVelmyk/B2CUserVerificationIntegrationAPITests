using System.Net;

namespace WX.B2C.User.Verification.Component.Tests.Constants
{
    internal static class IpAddresses
    {
        public const string TestIpHeaderKey = "TestIpAddress";

        public static readonly string FailAddress = IPAddress.None.ToString();
        public const string GbIpAddress = "2.103.254.255";
        public const string UsIpAddress = "101.198.198.45";
        public const string FrIpAddress = "103.112.128.255";
        public const string AuIpAddress = "1.0.4.0";
        public const string UaIpAddress = "103.197.148.0";
        public const string DzIpAddress = "102.218.188.0";
        public const string RuIpAddress = "104.18.47.93";
    }
}
