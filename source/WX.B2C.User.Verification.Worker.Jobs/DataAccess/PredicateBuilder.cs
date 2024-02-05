using System;
using System.Text;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;

namespace WX.B2C.User.Verification.Worker.Jobs.DataAccess
{
    internal enum BracketType
    {
        None = 1,
        Opened,
        Closed,
        Both
    }

    internal class PredicateBuilder
    {
        private readonly StringBuilder _stringBuilder;

        private PredicateBuilder(string predicate)
        {
            if(predicate == null) throw new ArgumentNullException(nameof(predicate));

            _stringBuilder = new StringBuilder(predicate);
        }

        public static PredicateBuilder Empty() =>
            new PredicateBuilder(string.Empty);

        public static PredicateBuilder With(string predicate, BracketType bracket = BracketType.None) =>
            Empty().InsertPredicate(string.Empty, predicate, bracket);

        public PredicateBuilder And(string predicate, BracketType bracket = BracketType.None) =>
            InsertPredicate("AND ", predicate, bracket);

        public PredicateBuilder Or(string predicate, BracketType bracket = BracketType.None) =>
            InsertPredicate("OR ", predicate, bracket);

        public string Build() =>
            _stringBuilder.ToString();

        private PredicateBuilder InsertPredicate(string @operator, string predicate, BracketType bracket)
        {
            _ = bracket switch
            {
                BracketType.None => _stringBuilder.AppendString(@operator).Append(predicate),
                BracketType.Opened => _stringBuilder.AppendString(@operator).Append("(").Append(predicate),
                BracketType.Closed => _stringBuilder.AppendString(@operator).Append(predicate).Append(")"),
                BracketType.Both => _stringBuilder.AppendString(@operator).Append("(").Append(predicate).Append(")"),
                _ => throw new ArgumentOutOfRangeException(nameof(bracket), bracket, "Invalid bracket type.")
            };
            return this;
        }
    }
}
