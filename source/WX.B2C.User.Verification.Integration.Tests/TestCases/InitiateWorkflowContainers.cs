using System;
using System.Collections;
using System.Runtime.CompilerServices;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.TestCases
{
    public class InitiateWorkflowContainers : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return Сvi50();
            yield return Сvi50Hri80();
            yield return Сvi50Hri11();
            yield return Сvi30();
            yield return Сvi10();
            yield return Сvi50NonResident();
            yield return Сvi30NonResident();
            yield return Сvi10NonResident();
        }

        private static InitiateWorkflowContainer Сvi50()
        {
            var dob = DateTime.Parse(HardcodedPersonalData.ResidentSuccessDob);
            var address = new Address()
            {
                Context = "primary",
                StreetAddress1 = HardcodedPersonalData.ResidentSuccessAddressLine,
                City = HardcodedPersonalData.ResidentSuccessCity,
                State = HardcodedPersonalData.ResidentSuccessState,
                Zip5 = HardcodedPersonalData.ResidentSuccessZipCode,
            };
            var dateOfBirth = new DateOfBirth
            {
                Year = dob.Year.ToString(),
                Month = dob.Month.ToString(),
                Day = dob.Day.ToString()
            };
            var personName = new PersonName
            {
                FirstName = HardcodedPersonalData.ResidentSuccessFirstName,
                LastName = HardcodedPersonalData.ResidentSuccessLastName
            };
            var person = new Person()
            {
                Context = "primary",
                Name = personName,
                Addresses = new[] { address },
                SSN = new Ssn
                {
                    Number = HardcodedPersonalData.ResidentSuccessSsn,
                    Type = "ssn9"
                },
                DateOfBirth = dateOfBirth
            };
            return Create(person, HardcodedPersonalData.ResidentSuccessCvi);
        }

        private static InitiateWorkflowContainer Create(Person person, int cvi, string hri = null, [CallerMemberName] string name = null)
        {
            var request = new InitiateWorkflowRequest
            {
                Type = "Initiate",
                Settings = null,
                Persons = new[] { person }
            };
            return new InitiateWorkflowContainer
            {
                Request = request,
                Name = name,
                Cvi = cvi,
                Hri = hri
            };
        }

        private InitiateWorkflowContainer Сvi50Hri80()
        {
            var dob = DateTime.Parse(HardcodedPersonalData.ResidentSuccessWithHriDob);
            var address = new Address()
            {
                Context = "primary",
                StreetAddress1 = HardcodedPersonalData.ResidentSuccessWithHriAddressLine,
                City = HardcodedPersonalData.ResidentSuccessWithHriCity,
                State = HardcodedPersonalData.ResidentSuccessWithHriState,
                Zip5 = HardcodedPersonalData.ResidentSuccessWithHriZipCode
            };
            var dateOfBirth = new DateOfBirth
            {
                Year = dob.Year.ToString(),
                Month = dob.Month.ToString(),
                Day = dob.Day.ToString()
            };
            var personName = new PersonName
            {
                FirstName = HardcodedPersonalData.ResidentSuccessWithHriFirstName,
                LastName = HardcodedPersonalData.ResidentSuccessWithHriLastName
            };
            var person = new Person()
            {
                Context = "primary",
                Name = personName,
                Addresses = new[] { address },
                SSN = new Ssn
                {
                    Number = HardcodedPersonalData.ResidentSuccessWithHriSsn,
                    Type = "ssn9"
                },
                DateOfBirth = dateOfBirth
            };

            return Create(person, HardcodedPersonalData.ResidentSuccessWithHriCvi, HardcodedPersonalData.ResidentSuccessWithHri);
        }

        private InitiateWorkflowContainer Сvi50Hri11()
        {
            var dob = DateTime.Parse(HardcodedPersonalData.ResidentPartialSuccessWithHriDob);
            var address = new Address()
            {
                Context = "primary",
                StreetAddress1 = HardcodedPersonalData.ResidentPartialSuccessWithHriAddressLine,
                City = HardcodedPersonalData.ResidentPartialSuccessWithHriCity,
                State = HardcodedPersonalData.ResidentPartialSuccessWithHriState,
                Zip5 = HardcodedPersonalData.ResidentPartialSuccessWithHriZipCode,
            };
            var dateOfBirth = new DateOfBirth
            {
                Year = dob.Year.ToString(),
                Month = dob.Month.ToString(),
                Day = dob.Day.ToString()
            };
            var personName = new PersonName
            {
                FirstName = HardcodedPersonalData.ResidentPartialSuccessWithHriFirstName,
                LastName = HardcodedPersonalData.ResidentPartialSuccessWithHriLastName
            };
            var person = new Person()
            {
                Context = "primary",
                Name = personName,
                Addresses = new[] { address },
                SSN = new Ssn
                {
                    Number = HardcodedPersonalData.ResidentPartialSuccessWithHriSsn,
                    Type = "ssn9"
                },
                DateOfBirth = dateOfBirth
            };
            return Create(person, HardcodedPersonalData.ResidentPartialSuccessWithHriCvi, HardcodedPersonalData.ResidentPartialSuccessWithHri);
        }

        private InitiateWorkflowContainer Сvi30()
        {
            var dob = DateTime.Parse(HardcodedPersonalData.ResidentPartialSuccessDob);
            var address = new Address()
            {
                Context = "primary",
                StreetAddress1 = HardcodedPersonalData.ResidentPartialSuccessAddressLine,
                City = HardcodedPersonalData.ResidentPartialSuccessCity,
                State = HardcodedPersonalData.ResidentPartialSuccessState,
                Zip5 = HardcodedPersonalData.ResidentPartialSuccessZipCode,
            };
            var dateOfBirth = new DateOfBirth
            {
                Year = dob.Year.ToString(),
                Month = dob.Month.ToString(),
                Day = dob.Day.ToString()
            };
            var personName = new PersonName
            {
                FirstName = HardcodedPersonalData.ResidentPartialSuccessFirstName,
                LastName = HardcodedPersonalData.ResidentPartialSuccessLastName
            };
            var person = new Person()
            {
                Context = "primary",
                Name = personName,
                Addresses = new[] { address },
                SSN = new Ssn
                {
                    Number = HardcodedPersonalData.ResidentPartialSuccessSsn,
                    Type = "ssn9"
                },
                DateOfBirth = dateOfBirth
            };
            return Create(person, HardcodedPersonalData.ResidentPartialSuccessCvi);
        }

        private InitiateWorkflowContainer Сvi10()
        {
            var dob = DateTime.Parse(HardcodedPersonalData.ResidentFailDob);
            var address = new Address()
            {
                Context = "primary",
                StreetAddress1 = HardcodedPersonalData.ResidentFailAddressLine,
                City = HardcodedPersonalData.ResidentFailCity,
                State = HardcodedPersonalData.ResidentFailState,
                Zip5 = HardcodedPersonalData.ResidentFailZipCode,
            };
            var dateOfBirth = new DateOfBirth
            {
                Year = dob.Year.ToString(),
                Month = dob.Month.ToString(),
                Day = dob.Day.ToString()
            };
            var personName = new PersonName
            {
                FirstName = HardcodedPersonalData.ResidentFailFirstName,
                LastName = HardcodedPersonalData.ResidentFailLastName
            };
            var person = new Person()
            {
                Context = "primary",
                Name = personName,
                Addresses = new[] { address },
                SSN = new Ssn
                {
                    Number = HardcodedPersonalData.ResidentFailSsn,
                    Type = "ssn9"
                },
                DateOfBirth = dateOfBirth
            };
            return Create(person, HardcodedPersonalData.ResidentFailCvi);
        }

        private InitiateWorkflowContainer Сvi50NonResident()
        {
            var dob = DateTime.Parse(HardcodedPersonalData.NonResidentSuccessDob);
            var address = new Address()
            {
                Context = "primary",
                StreetAddress1 = HardcodedPersonalData.NonResidentSuccessAddressLine,
                City = HardcodedPersonalData.NonResidentSuccessCity,
                State = HardcodedPersonalData.NonResidentSuccessState,
                Zip5 = HardcodedPersonalData.NonResidentSuccessZipCode,
            };
            var dateOfBirth = new DateOfBirth
            {
                Year = dob.Year.ToString(),
                Month = dob.Month.ToString(),
                Day = dob.Day.ToString()
            };
            var personName = new PersonName
            {
                FirstName = HardcodedPersonalData.NonResidentSuccessFirstName,
                LastName = HardcodedPersonalData.NonResidentSuccessLastName
            };
            var person = new Person()
            {
                Context = "primary",
                Name = personName,
                Addresses = new[] { address },
                DateOfBirth = dateOfBirth
            };
            return Create(person, HardcodedPersonalData.NonResidentSuccessCvi);
        }

        private InitiateWorkflowContainer Сvi30NonResident()
        {
            var dob = DateTime.Parse(HardcodedPersonalData.NonResidentPartialSuccessDob);
            var address = new Address()
            {
                Context = "primary",
                StreetAddress1 = HardcodedPersonalData.NonResidentPartialSuccessAddressLine,
                City = HardcodedPersonalData.NonResidentPartialSuccessCity,
                State = HardcodedPersonalData.NonResidentPartialSuccessState,
                Zip5 = HardcodedPersonalData.NonResidentPartialSuccessZipCode,
            };
            var dateOfBirth = new DateOfBirth
            {
                Year = dob.Year.ToString(),
                Month = dob.Month.ToString(),
                Day = dob.Day.ToString()
            };
            var personName = new PersonName
            {
                FirstName = HardcodedPersonalData.NonResidentPartialSuccessFirstName,
                LastName = HardcodedPersonalData.NonResidentPartialSuccessLastName
            };
            var person = new Person()
            {
                Context = "primary",
                Name = personName,
                Addresses = new[] { address },
                DateOfBirth = dateOfBirth
            };
            return Create(person, HardcodedPersonalData.NonResidentPartialSuccessCvi);
        }

        private InitiateWorkflowContainer Сvi10NonResident()
        {
            var dob = DateTime.Parse(HardcodedPersonalData.NonResidentFailDob);
            var address = new Address()
            {
                Context = "primary",
                StreetAddress1 = HardcodedPersonalData.NonResidentFailAddressLine,
                City = HardcodedPersonalData.NonResidentFailCity,
                State = HardcodedPersonalData.NonResidentFailState,
                Zip5 = HardcodedPersonalData.NonResidentFailZipCode,
            };
            var dateOfBirth = new DateOfBirth
            {
                Year = dob.Year.ToString(),
                Month = dob.Month.ToString(),
                Day = dob.Day.ToString()
            };
            var personName = new PersonName
            {
                FirstName = HardcodedPersonalData.NonResidentFailFirstName,
                LastName = HardcodedPersonalData.NonResidentFailLastName
            };
            var person = new Person()
            {
                Context = "primary",
                Name = personName,
                Addresses = new[] { address },
                DateOfBirth = dateOfBirth
            };
            return Create(person, HardcodedPersonalData.NonResidentFailCvi);
        }
    }
}