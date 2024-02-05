using FsCheck;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders
{
    internal class InitiateWorkflowRequestArbitrary : Arbitrary<InitiateWorkflowRequest>
    {
        private const string Locale = "en_US";
        private const string Country = "US";

        public static Arbitrary<InitiateWorkflowRequest> Create() => new InitiateWorkflowRequestArbitrary();

        public override Gen<InitiateWorkflowRequest> Generator =>
            from seed in Arb.Generate<int>()
            from firstName in PersonalDetailsGenerator.FirstName(seed, Locale)
            from lastName in PersonalDetailsGenerator.LastName(seed, Locale)
            from dob in DateTimeGenerators.BeforeUtcNow(-18)
            from ssnNumber in StringGenerators.Numbers(9)
            from address_ in AddressGenerator.Address(seed, Locale)
            let address = new Address
            {
                Context = "primary",
                StreetAddress1 = address_.Line1,
                StreetAddress2 = address_.Line2,
                City = address_.City,
                State = address_.State,
                Zip5 = address_.ZipCode,
                Country = Country
            }
            let dateOfBirth = new DateOfBirth
            {
                Year = dob.Year.ToString(),
                Month = dob.Month.ToString(),
                Day = dob.Day.ToString()
            }
            let personName = new PersonName
            {
                FirstName = firstName,
                LastName = lastName
            }
            let person = new Person
            {
                Context = "primary",
                Name = personName,
                Addresses = new[] { address },
                SSN = new Ssn
                {
                    Number = ssnNumber,
                    Type = "ssn9"
                },
                DateOfBirth = dateOfBirth
            }
            select new InitiateWorkflowRequest
            {
                Type = "Initiate",
                Settings = null,
                Persons = new[] { person }
            };
    }
}