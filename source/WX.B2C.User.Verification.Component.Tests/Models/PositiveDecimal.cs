using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal sealed class PositiveDecimal
    {
        public decimal Value { get; set; }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || Equals(obj as PositiveDecimal);

        private bool Equals(PositiveDecimal other) =>
            other is not null && Value.IsEquivalent(other.Value);

        public override int GetHashCode() => decimal.Truncate(Value).GetHashCode();

        public static implicit operator decimal(PositiveDecimal pd) => pd.Value;
    }
}
