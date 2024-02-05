using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.PassFort.Exceptions
{
    [Serializable]
    public class PassFortEntityNotFoundException : PassFortApiException
    {
        public PassFortEntityNotFoundException(string profileId, string entityType)
            : base($"Entity on PassFort not found. ProfileId:{profileId}, EntityType:{entityType}")
        {
        }

        public PassFortEntityNotFoundException(string profileId, TaskType type)
            : base($"Task on PassFort not found. ProfileId:{profileId}, TaskType:{type}")
        {
        }

        protected PassFortEntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}