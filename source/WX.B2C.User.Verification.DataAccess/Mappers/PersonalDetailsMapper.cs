using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IPersonalDetailsMapper
    {
        PersonalDetailsDto Map(PersonalDetails entity);

        PersonalDetails Map(PersonalDetailsDto dto);

        AddressDto SafeMap(ResidenceAddress residenceAddress);

        void Update(PersonalDetailsDto dto, PersonalDetails entity);
    }

    internal class PersonalDetailsMapper : IPersonalDetailsMapper
    {
        public PersonalDetailsDto Map(PersonalDetails entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var residenceAddress = SafeMap(entity.ResidenceAddress);

            return new PersonalDetailsDto
            {
                UserId = entity.UserId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                DateOfBirth = entity.DateOfBirth,
                Nationality = entity.Nationality,
                Email = entity.Email,
                CreatedAt = entity.CreatedAt,
                ResidenceAddress = residenceAddress
            };
        }

        public PersonalDetails Map(PersonalDetailsDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var entity = new PersonalDetails { UserId = dto.UserId };
            Update(dto, entity);
            return entity;
        }

        public void Update(PersonalDetailsDto dto, PersonalDetails entity)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var residenceAddress = SafeMap(dto.ResidenceAddress, dto.UserId);

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.DateOfBirth = dto.DateOfBirth;
            entity.Nationality = dto.Nationality;
            entity.Email = dto.Email;
            entity.ResidenceAddress = residenceAddress;
        }

        public AddressDto SafeMap(ResidenceAddress residenceAddress)
        {
            if (residenceAddress == null) return null;

            return new AddressDto
            {
                Line1 = residenceAddress.Line1,
                Line2 = residenceAddress.Line2,
                City = residenceAddress.City,
                State = residenceAddress.State,
                Country = residenceAddress.Country,
                ZipCode = residenceAddress.ZipCode
            };
        }

        private static ResidenceAddress SafeMap(AddressDto addressDto, Guid userId)
        {
            if (addressDto == null) return null;

            return new ResidenceAddress
            {
                UserId = userId,
                Line1 = addressDto.Line1,
                Line2 = addressDto.Line2,
                City = addressDto.City,
                State = addressDto.State,
                Country = addressDto.Country,
                ZipCode = addressDto.ZipCode
            };
        }
    }
}