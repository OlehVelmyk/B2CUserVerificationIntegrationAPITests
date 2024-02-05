using System;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.PassFort.Exceptions
{
    [Serializable]
    public class PassFortCannotUpdateProfileException : PassFortApiException
    {
        public PassFortCannotUpdateProfileException(string profileId)
            : base($"Profile is updated in parallel. Etag is outdated. Retry required. profileId: {profileId}")
        {
        }

        protected PassFortCannotUpdateProfileException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
