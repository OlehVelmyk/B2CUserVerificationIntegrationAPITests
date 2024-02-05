using FsCheck;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders
{
    internal class OnfidoApplicantArbitrary : Arbitrary<NewApplicant>
    {
        public static Arbitrary<NewApplicant> Create() => new OnfidoApplicantArbitrary();

        public override Gen<NewApplicant> Generator =>
            from seed in Arb.Generate<int>()
            from firstName in PersonalDetailsGenerator.FirstName(seed)
            from lastName in PersonalDetailsGenerator.LastName(seed)
            from email in PersonalDetailsGenerator.Email(seed)
            select new NewApplicant
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };
    }

    internal class OnfidoApplicantWithForbiddenCharactersArbitrary : Arbitrary<OnfidoApplicantWithForbiddenCharacters>
    {
        private static readonly char[] _forbiddenCharacters = "^!#$%*=<>;{}\"".ToCharArray();

        public static Arbitrary<OnfidoApplicantWithForbiddenCharacters> Create() =>
            new OnfidoApplicantWithForbiddenCharactersArbitrary();

        public override Gen<OnfidoApplicantWithForbiddenCharacters> Generator =>
            from seed in Arb.Generate<int>()
            from firstName in PersonalDetailsGenerator.FirstName(seed)
            from lastName in PersonalDetailsGenerator.LastName(seed)
            from email in PersonalDetailsGenerator.Email(seed)
            from twoChars in Gen.Two(ForbiddenCharacters)
            from position1 in Gen.Choose(0, firstName.Length - 1)
            from position2 in Gen.Choose(0, lastName.Length - 1)
            select new OnfidoApplicantWithForbiddenCharacters
            {
                FirstName = firstName.Insert(position1, twoChars.Item1.ToString()),
                LastName = lastName.Insert(position2, twoChars.Item2.ToString()),
                Email = email
            };

        private static Gen<char> ForbiddenCharacters => Gen.Elements(_forbiddenCharacters);
    }
}
