using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Profile
{
    public sealed class AddressDto
    {
        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        private bool Equals(AddressDto other)
        {
            return Line1 == other.Line1 && Line2 == other.Line2 && City == other.City && State == other.State && Country == other.Country && ZipCode == other.ZipCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return Equals((AddressDto) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Line1, Line2, City, State, Country, ZipCode);
        }

        public static bool operator ==(AddressDto left, AddressDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AddressDto left, AddressDto right)
        {
            return !Equals(left, right);
        }
    }
}