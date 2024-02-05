using System.Text.Json;
using System.Text.RegularExpressions;

namespace WX.B2C.User.Verification.Api.Public.Helpers
{
    internal class SnakeCaseJsonNamingPolicy : JsonNamingPolicy
    {
        public static JsonNamingPolicy Instance { get; } = new SnakeCaseJsonNamingPolicy();

        public override string ConvertName(string name)
        {
            return ToSnakeCase(name);
        }

        private static string ToSnakeCase(string name) => Regex.Replace(name, "([a-z])([A-Z])", "$1_$2").ToLower();
    }
}