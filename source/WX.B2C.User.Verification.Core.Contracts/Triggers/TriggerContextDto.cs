using System.Collections.Generic;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Profile;

namespace WX.B2C.User.Verification.Core.Contracts.Triggers
{
    [KnownType(typeof(RiskLevel))]
    [KnownType(typeof(string[]))]
    [KnownType(typeof(TinDto))]
    public class TriggerContextDto : Dictionary<string, object>
    {
        public TriggerContextDto()
        {
            
        }

        private TriggerContextDto(IDictionary<string,object> context):base(context)
        {
            
        }

        public static TriggerContextDto Create(Dictionary<string, object> context)
        {
            return new(context);
        }
    }
}