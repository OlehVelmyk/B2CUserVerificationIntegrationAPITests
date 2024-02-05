using System;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public abstract class CollectionStepException : B2CVerificationException
    {
        protected CollectionStepException(Guid id, string xPath, string message)
            : base(message)
        {
            Id = id.ToString();
            XPath = xPath;
        }

        protected CollectionStepException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            Id = info.GetString(nameof(Id));
            XPath = info.GetString(nameof(XPath));
        }

        public string Id { get; set; }

        public string XPath { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(XPath), XPath);

            base.GetObjectData(info, context);
        }
    }
}