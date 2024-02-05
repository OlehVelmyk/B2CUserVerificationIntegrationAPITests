using Newtonsoft.Json;

namespace WX.B2C.User.Verification.Automation.Services.Extensions
{
    internal static class SerializationExtensions
    {
        public static T To<T>(this object value) where T : class
        {
            return JsonConvert.DeserializeObject<T>(value.ToString());
        }
    }
}