using System;
using BridgerReference;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.LexisNexis.Models;
using WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models;

namespace WX.B2C.User.Verification.LexisNexis.Mappers
{
    internal interface ICheckRequestMapper
    {
        Person Map(FraudScreeningCheckData inputData);

        SearchInput Map(RiskScreeningCheckData inputData);
    }

    internal class CheckRequestMapper : ICheckRequestMapper
    {
        public Person Map(FraudScreeningCheckData inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException(nameof(inputData));

            var birthDate = inputData.BirthDate;
            var residenceAddress = inputData.Address;
            var ssn = inputData.Tin.Type == TinType.SSN
                ? new Ssn
                {
                    Number = inputData.Tin.Number,
                    Type = "ssn9"
                }
                : null;

            var address = new Address
            {
                Context = "primary",
                StreetAddress1 = residenceAddress.Line1,
                StreetAddress2 = residenceAddress.Line2,
                City = residenceAddress.City,
                State = residenceAddress.State,
                Zip5 = residenceAddress.ZipCode,
                Country = residenceAddress.Country
            };
            var dateOfBirth = new DateOfBirth
            {
                Year = birthDate.Year.ToString(),
                Month = birthDate.Month.ToString(),
                Day = birthDate.Day.ToString()
            };
            var personName = new PersonName
            {
                FirstName = inputData.FullName.FirstName,
                LastName = inputData.FullName.LastName
            };

            return new Person
            {
                Context = "primary",
                Name = personName,
                Addresses = new[] { address },
                SSN = ssn,
                DateOfBirth = dateOfBirth
            };
        }

        public SearchInput Map(RiskScreeningCheckData inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException(nameof(inputData));

            var name = new InputName
            {
                First = inputData.FullName.FirstName,
                Last = inputData.FullName.LastName
            };
            var address = new InputAddress
            {
                Type = AddressType.Current,
                Country = inputData.Address.Country,
                PostalCode = inputData.Address.ZipCode,
                City = inputData.Address.City,
                StateProvinceDistrict = inputData.Address.State,
                Street1 = inputData.Address.Line1,
                Street2 = inputData.Address.Line2
            };
            var dateOfBirth = new Date
            {
                Day = inputData.BirthDate.Day,
                Month = inputData.BirthDate.Month,
                Year = inputData.BirthDate.Year
            };
            var dobInfo = new InputAdditionalInfo
            {
                Type = AdditionalInfoType.DOB,
                Date = dateOfBirth
            };
            var emailInfo = new InputAdditionalInfo
            {
                Type = AdditionalInfoType.Other,
                Label = "Email",
                Value = inputData.Email
            };
            var additionalInfo = inputData.Email is null 
                ? new[] { dobInfo }
                : new[] { dobInfo, emailInfo };
            var tin = new InputID
            {
                Type = IDType.SSN,
                Number = inputData.Tin.Number
            };
            var inputRecord = new InputRecord
            {
                Entity = new InputEntity
                {
                    EntityType = InputEntityType.Individual,
                    Gender = GenderType.Unknown,
                    Name = name,
                    AdditionalInfo = additionalInfo,
                    Addresses = new[] { address },
                    IDs = new[] { tin }
                }
            };
            return new SearchInput
            {
                Records = new[] { inputRecord }
            };
        }
    }
}
