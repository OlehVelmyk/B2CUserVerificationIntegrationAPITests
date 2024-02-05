using System;
using System.Linq;
using FsCheck;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Extensions;
using PersonalDetailsProperty = WX.B2C.User.Verification.Component.Tests.Models.PersonalDetailsProperty;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal static class PersonalDetailsPropertyGenerator
    {
        private static readonly PersonalDetailsProperty[] Properties =
        {
            new(
                nameof(PersonalDetails.FirstName),
                dto => dto.FirstName,
                (info, _) => info.FirstName,
                changes => changes.Find<object>(PersonalDetails.FirstName)
            ),
            new(
                nameof(PersonalDetails.LastName),
                dto => dto.LastName,
                (info, _) => info.LastName,
                changes => changes.Find<object>(PersonalDetails.LastName)
            ),
            new(
                nameof(PersonalDetails.Birthdate),
                dto => dto.DateOfBirth,
                (info, _) => info.DateOfBirth.Date,
                changes => changes.Find<object>(PersonalDetails.Birthdate)
            ),
            new(
                nameof(PersonalDetails.Nationality),
                dto => dto.Nationality,
                (info, _) => info.Nationality,
                changes => changes.Find<object>(PersonalDetails.Nationality)
            ),
            new(
                nameof(PersonalDetails.Email),
                dto => dto.Email,
                (info, _) => info.Email,
                changes => changes.Find<object>(PersonalDetails.Email)
            ),
            new(
                nameof(PersonalDetails.ResidenceAddress),
                dto => dto.ResidenceAddress,
                (info, _) => info.Address,
                changes =>
                {
                    var change = changes.Find<Address>(PersonalDetails.ResidenceAddress);
                    return change is null ? null : new PropertyChange<object>(change.NewValue, change.PreviousValue);
                }
            )
        };

        public static Gen<PersonalDetailsProperty[]> Generate() =>
            from length in Gen.Choose(1, Properties.Length)
            from properties in Gen.ArrayOf(length, Gen.Elements(Properties))
            select AddFullNameChange(properties);

        private static PersonalDetailsProperty[] AddFullNameChange(PersonalDetailsProperty[] properties)
        {
            var isFirstNameExists = properties.FirstOrDefault(property => property.Type is nameof(PersonalDetails.FirstName)) is not null;
            var isLastNameExists = properties.FirstOrDefault(property => property.Type is nameof(PersonalDetails.LastName)) is not null;

            Func<UserInfo, PersonalDetailsDto, object> expectedValueSelector = null;
            if (isFirstNameExists)
                expectedValueSelector = (info, dto) => FullName.Create(info.FirstName, dto.LastName);

            if (isLastNameExists)
                expectedValueSelector = (info, dto) => FullName.Create(dto.FirstName, info.LastName);

            if (isFirstNameExists && isLastNameExists)
                expectedValueSelector = (info, _) => FullName.Create(info.FirstName, info.LastName);

            if (expectedValueSelector is not null)
                return properties.Concat(new[]
                {
                    new PersonalDetailsProperty(
                        nameof(PersonalDetails.FullName),
                        dto => FullName.Create(dto.FirstName, dto.LastName),
                        expectedValueSelector,
                        changes =>
                        {
                            var change = changes.Find<FullName>(PersonalDetails.FullName);
                            return change is null ? null : new PropertyChange<object>(change.NewValue, change.PreviousValue);
                        })
                }).ToArray();

            return properties;
        }
    }
}
