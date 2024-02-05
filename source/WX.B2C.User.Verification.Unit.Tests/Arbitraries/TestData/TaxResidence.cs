using System.Linq;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData
{
    public class TaxResidence
    {
        public TaxResidence(bool hasValue, string[] countries)
        {
            HasValue = hasValue;
            if (hasValue)
                Value = countries.Distinct().ToArray();
        }

        public bool HasValue { get; }

        public string[] Value { get; }
    }
}