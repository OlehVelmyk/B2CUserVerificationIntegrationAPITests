using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Models
{
    public class ResidenceAddress : ValueObject
    {
        /// <summary>
        /// ISO 3166-1 two letter code of the country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Two letter code of state.
        /// </summary>
        public string State { get; set; }

        public string City { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string ZipCode { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Country;
            yield return State;
            yield return City;
            yield return Line1;
            yield return Line2;
            yield return ZipCode;
        }
    }
}
