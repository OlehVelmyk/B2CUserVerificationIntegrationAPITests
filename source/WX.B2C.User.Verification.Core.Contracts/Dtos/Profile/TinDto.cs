using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Profile
{
    public class TinDto
    {
        public string Number { get; set; }

        public TinType Type { get; set; }

        protected bool Equals(TinDto other)
        {
            return Number == other.Number && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is TinDto other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, (int) Type);
        }

        public static bool operator ==(TinDto left, TinDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TinDto left, TinDto right)
        {
            return !Equals(left, right);
        }
    }
}