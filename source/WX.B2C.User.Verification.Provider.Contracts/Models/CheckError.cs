using System;
using System.Collections.Generic;

namespace WX.B2C.User.Verification.Provider.Contracts.Models
{
    [Serializable]
    public class CheckError
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public IDictionary<string, object> AdditionalData { get; set; }

        public static CheckError Create(string code, string message, IDictionary<string, object> additionalData = null)
        {
            return new CheckError
            {
                Code = code,
                Message = message,
                AdditionalData = additionalData
            };
        }

        public CheckError WithAdditionalData(string name, object data)
        {
            AdditionalData ??= new Dictionary<string, object>();
            AdditionalData.Add(name, data);
            return this;
        }
    }
}