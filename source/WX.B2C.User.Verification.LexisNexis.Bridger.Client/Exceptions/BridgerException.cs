using System;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.LexisNexis.Bridger.Client.Exceptions
{
    [Serializable]
    public class BridgerException : Exception
    {
        public string Description { get; }

        public string ErrorMessage { get; set; }

        public BridgerException()
        {
        }

        public BridgerException(string message)
            : base(message)
        {
        }

        public BridgerException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }

        public BridgerException(string description, string errorMessage, Exception innerException = null)
            : this($"Description: {description}, Message: {errorMessage}", innerException)
        {
            Description = description;
            ErrorMessage = errorMessage;
        }

        public BridgerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            Description = info.GetString(nameof(Description));
            ErrorMessage = info.GetString(nameof(ErrorMessage));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Description), Description);
            info.AddValue(nameof(ErrorMessage), ErrorMessage);

            base.GetObjectData(info, context);
        }

        public override string ToString()
        {
            return $"Description: {Description}, Message: {ErrorMessage}, {base.ToString()}";
        }
    }
}
