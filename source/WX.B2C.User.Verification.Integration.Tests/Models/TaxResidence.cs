using System.Linq;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    public class TaxResidence
    {
        public TaxResidence(bool hasValue, string[] countries)
        {
            if (hasValue)
                Value = countries.Distinct().ToArray();
        }

        public string[] Value { get; }
    }
}