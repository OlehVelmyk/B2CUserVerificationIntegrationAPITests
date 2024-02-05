using System;
using Newtonsoft.Json;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services
{
    internal class CheckOutputDataSerializer : ICheckOutputDataSerializer
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings();

        public string Serialize(CheckOutputData outputData)
        {
            if (outputData == null) return null;

            return JsonConvert.SerializeObject(outputData, _settings);
        }

        public TData Deserialize<TData>(string jsonData) where TData : CheckOutputData
        {
            if (string.IsNullOrEmpty(jsonData)) 
                throw new ArgumentNullException(nameof(jsonData));

            return JsonConvert.DeserializeObject<TData>(jsonData, _settings);
        }
    }
}
