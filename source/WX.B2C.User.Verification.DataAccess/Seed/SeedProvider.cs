using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WX.B2C.User.Verification.DataAccess.Seed
{
    internal static class SeedProvider
    {
        internal static T[] GetEntities<T>(string filePath)
        {
            if (!File.Exists(filePath))
                return Array.Empty<T>();

            try
            {
                var result = File.ReadAllText(filePath);
                return Deserialize<T>(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Array.Empty<T>();
            }
        }

        private static T[] Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T[]>(json, new StringEnumConverter());
        }
    }
}
