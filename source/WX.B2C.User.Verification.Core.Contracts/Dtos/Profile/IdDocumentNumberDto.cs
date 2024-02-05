using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Profile
{
    public class IdDocumentNumberDto
    {
        public string Number { get; set; }

        public string Type { get; set; }

        public static IdDocumentNumberDto NotPresented => new() { Number = "NOT PRESENTED" };

        protected bool Equals(IdDocumentNumberDto other)
        {
            return Number == other.Number && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;

            return Equals((IdDocumentNumberDto) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, Type);
        }

        public static bool operator ==(IdDocumentNumberDto left, IdDocumentNumberDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IdDocumentNumberDto left, IdDocumentNumberDto right)
        {
            return !Equals(left, right);
        }
    }
}