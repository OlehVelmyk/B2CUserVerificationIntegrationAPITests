using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData;

namespace WX.B2C.User.Verification.Unit.Tests.DataAccess
{
    internal class ExpressionExtensionsTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ArrayArbitrary<PersonalDetailsDto>.Initialize(10, 20);
            Arb.Register<ArrayArbitrary<PersonalDetailsDto>>();
        }

        [Theory]
        public void ShouldFindOneByAndOperation(PersonalDetailsDto[] personalDetailsArray, uint number)
        {
            // Arrange
            var index = number % personalDetailsArray.Length;
            var personalDetails = personalDetailsArray[index];
            Expression<Func<PersonalDetailsDto, bool>> filterByFirstName = x => x.FirstName == personalDetails.FirstName;
            Expression<Func<PersonalDetailsDto, bool>> filterByLastName = x => x.LastName == personalDetails.LastName;

            // Act
            var predicate = filterByFirstName.And(filterByLastName);
            var actual = personalDetailsArray.AsQueryable().Where(predicate).ToArray();

            // Assert
            actual.Should().OnlyContain(x => x.FirstName == personalDetails.FirstName && x.LastName == personalDetails.LastName);
        }

        [Theory]
        public void Or_ShouldCorrectlyCombineExpressions(Two<PersonalDetailsDto> personalDetails)
        {
            var (personalDetails1, personalDetails2) = personalDetails;

            // Arrange
            Expression<Func<PersonalDetailsDto, bool>> filterById = x => x.UserId == personalDetails1.UserId;
            Expression<Func<PersonalDetailsDto, bool>> filterByEmail = x => x.Email == personalDetails2.Email;

            // Act
            var predicate = filterById.Or(filterByEmail);

            // Assert
            Expression<Func<PersonalDetailsDto, bool>> expected = x =>
                x.UserId == personalDetails1.UserId ||
                x.Email == personalDetails2.Email;

            predicate.Should().BeEquivalentTo(expected, options => options.Using(new ExpressionEqualityComparer()));
        }

        [Theory]
        public void And_ShouldCorrectlyCombineExpressions(Two<PersonalDetailsDto> personalDetails)
        {
            var (personalDetails1, personalDetails2) = personalDetails;

            // Arrange
            Expression<Func<PersonalDetailsDto, bool>> filterById = x => x.UserId == personalDetails1.UserId;
            Expression<Func<PersonalDetailsDto, bool>> filterByEmail = x => x.Email == personalDetails2.Email;
            
            // Act
            var predicate = filterById.And(filterByEmail);

            // Assert
            Expression<Func<PersonalDetailsDto, bool>> expected = x =>
                x.UserId == personalDetails1.UserId &&
                x.Email == personalDetails2.Email;

            predicate.Should().BeEquivalentTo(expected, options => options.Using(new ExpressionEqualityComparer()));
        }

        [Theory]
        public void OrAnd_ShouldCorrectlyCombineComplexExpressions(Two<PersonalDetailsDto> personalDetails)
        {
            var (personalDetails1, personalDetails2) = personalDetails;

            // Arrange
            Expression<Func<PersonalDetailsDto, bool>> filterById1 = x => x.UserId == personalDetails1.UserId;
            Expression<Func<PersonalDetailsDto, bool>> filterById2 = x => x.UserId == personalDetails2.UserId;

            Expression<Func<PersonalDetailsDto, bool>> filterByEmail1 = x => x.Email == personalDetails1.Email;
            Expression<Func<PersonalDetailsDto, bool>> filterByEmail2 = x => x.Email == personalDetails2.Email;

            // Act
            var predicate = filterById1.Or(filterById2).And(filterByEmail1.Or(filterByEmail2));

            // Assert
            Expression<Func<PersonalDetailsDto, bool>> expected = x =>
                (x.UserId == personalDetails1.UserId || x.UserId == personalDetails2.UserId) &&
                (x.Email == personalDetails1.Email || x.Email == personalDetails2.Email);

            predicate.Should().BeEquivalentTo(expected, options => options.Using(new ExpressionEqualityComparer()));
        }

        [Theory]
        public void AndOr_ShouldCorrectlyCombineComplexExpressions(Two<PersonalDetailsDto> personalDetails)
        {
            var (personalDetails1, personalDetails2) = personalDetails;

            // Arrange
            Expression<Func<PersonalDetailsDto, bool>> filterById1 = x => x.UserId == personalDetails1.UserId;
            Expression<Func<PersonalDetailsDto, bool>> filterById2 = x => x.UserId == personalDetails2.UserId;

            Expression<Func<PersonalDetailsDto, bool>> filterByEmail1 = x => x.Email == personalDetails1.Email;
            Expression<Func<PersonalDetailsDto, bool>> filterByEmail2 = x => x.Email == personalDetails2.Email;

            // Act
            var predicate = filterById1.And(filterByEmail1).Or(filterById2.And(filterByEmail2));

            // Assert
            Expression<Func<PersonalDetailsDto, bool>> expected = x =>
                x.UserId == personalDetails1.UserId && x.Email == personalDetails1.Email ||
                x.UserId == personalDetails2.UserId && x.Email == personalDetails2.Email;

            predicate.Should().BeEquivalentTo(expected, options => options.Using(new ExpressionEqualityComparer()));
        }
    }
}
