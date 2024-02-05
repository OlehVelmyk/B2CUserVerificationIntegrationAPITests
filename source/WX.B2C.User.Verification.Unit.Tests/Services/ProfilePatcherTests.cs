using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using Optional;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Services;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries;
using WX.B2C.User.Verification.Unit.Tests.Extensions;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Unit.Tests.Services
{
    internal class ProfilePatcherTests
    {
        private const string FirstNameXPath = "PersonalDetails.FirstName";
        private const string LastNameXPath = "PersonalDetails.LastName";
        private const string FullNameXPath = "PersonalDetails.FullName";
        private readonly IProfilePatcher _profilePatcher = new ProfilePatcher();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Arb.Register<PersonalDetailsArbitrary>();
        }

        [Theory]
        public void ShouldCreateFullNameChange_WhenFirstNameUpdated(PersonalDetailsDto model, NonEmptyString firstName)
        {
            // Arrange
            var firstNamePreviousValue = model.FirstName;
            var firstNameNewValue = firstName.Item;
            var patch = new PersonalDetailsPatch
            {
                FirstName = firstName.Item.Some()
            };

            // Act
            var result = _profilePatcher.ApplyPatch(model, patch);

            // Assert
            model.FirstName.Should().Be(firstNameNewValue);
            result.Length.Should().Be(2);

            var firstNameChange = result.Find<string>(FirstNameXPath);
            firstNameChange.Should().NotBeNull();
            firstNameChange.NewValue.Should().Be(firstNameNewValue);
            firstNameChange.PreviousValue.Should().Be(firstNamePreviousValue);

            var fullNameChange = result.Find<FullNameDto>(FullNameXPath);
            fullNameChange.Should().NotBeNull();
            fullNameChange.NewValue.FirstName.Should().Be(firstNameNewValue);
            fullNameChange.NewValue.LastName.Should().Be(model.LastName);
            fullNameChange.PreviousValue.FirstName.Should().Be(firstNamePreviousValue);
            fullNameChange.PreviousValue.LastName.Should().Be(model.LastName);
        }

        [Theory]
        public void ShouldCreateFullNameChange_WhenLastNameUpdated(PersonalDetailsDto model, NonEmptyString lastName)
        {
            // Arrange
            var lastNamePreviousValue = model.LastName;
            var lastNameNewValue = lastName.Item;

            var patch = new PersonalDetailsPatch
            {
                LastName = lastNameNewValue.Some()
            };

            // Act
            var result = _profilePatcher.ApplyPatch(model, patch);

            // Assert
            model.LastName.Should().Be(lastNameNewValue);
            result.Length.Should().Be(2);

            var lastNameChange = result.Find<string>(LastNameXPath);
            lastNameChange.Should().NotBeNull();
            lastNameChange.NewValue.Should().Be(lastNameNewValue);
            lastNameChange.PreviousValue.Should().Be(lastNamePreviousValue);

            var fullNameChange = result.Find<FullNameDto>(FullNameXPath);
            fullNameChange.Should().NotBeNull();
            fullNameChange.NewValue.FirstName.Should().Be(model.FirstName);
            fullNameChange.NewValue.LastName.Should().Be(lastNameNewValue);
            fullNameChange.PreviousValue.FirstName.Should().Be(model.FirstName);
            fullNameChange.PreviousValue.LastName.Should().Be(lastNamePreviousValue);
        }

        [Theory]
        public void ShouldCreateFullNameChange_WhenFirstAndLastNameUpdated(PersonalDetailsDto model, NonEmptyString firstName, NonEmptyString lastName)
        {
            // Arrange
            var firstNamePreviousValue = model.FirstName;
            var firstNameNewValue = firstName.Item;
            var lastNamePreviousValue = model.LastName;
            var lastNameNewValue = lastName.Item;

            var patch = new PersonalDetailsPatch
            {
                FirstName = firstNameNewValue.Some(),
                LastName = lastNameNewValue.Some()
            };

            // Act
            var result = _profilePatcher.ApplyPatch(model, patch);

            // Assert
            model.FirstName.Should().Be(firstNameNewValue);
            model.LastName.Should().Be(lastNameNewValue);
            result.Length.Should().Be(3);

            var firstNameChange = result.Find<string>(FirstNameXPath);
            firstNameChange.Should().NotBeNull();
            firstNameChange.NewValue.Should().Be(firstNameNewValue);
            firstNameChange.PreviousValue.Should().Be(firstNamePreviousValue);

            var lastNameChange = result.Find<string>(LastNameXPath);
            lastNameChange.Should().NotBeNull();
            lastNameChange.NewValue.Should().Be(lastNameNewValue);
            lastNameChange.PreviousValue.Should().Be(lastNamePreviousValue);

            var fullNameChange = result.Find<FullNameDto>(FullNameXPath);
            fullNameChange.Should().NotBeNull();
            fullNameChange.NewValue.FirstName.Should().Be(firstNameNewValue);
            fullNameChange.NewValue.LastName.Should().Be(lastNameNewValue);
            fullNameChange.PreviousValue.FirstName.Should().Be(firstNamePreviousValue);
            fullNameChange.PreviousValue.LastName.Should().Be(lastNamePreviousValue);
        }

        [Theory]
        public void ShouldCreateFullNameChange_WhenPreviousValueIsNull(PersonalDetailsDto model, NonEmptyString firstName, NonEmptyString lastName)
        {
            // Arrange
            model.FirstName = null;
            model.LastName = null;
            var firstNameNewValue = firstName.Item;
            var lastNameNewValue = lastName.Item;

            var patch = new PersonalDetailsPatch
            {
                FirstName = firstNameNewValue.Some(),
                LastName = lastNameNewValue.Some()
            };

            // Act
            var result = _profilePatcher.ApplyPatch(model, patch);

            // Assert
            model.FirstName.Should().Be(firstNameNewValue);
            model.LastName.Should().Be(lastNameNewValue);
            result.Length.Should().Be(3);

            var firstNameChange = result.Find<string>(FirstNameXPath);
            firstNameChange.Should().NotBeNull();
            firstNameChange.NewValue.Should().Be(firstNameNewValue);
            firstNameChange.PreviousValue.Should().BeNull();

            var lastNameChange = result.Find<string>(LastNameXPath);
            lastNameChange.Should().NotBeNull();
            lastNameChange.NewValue.Should().Be(lastNameNewValue);
            lastNameChange.PreviousValue.Should().BeNull();

            var fullNameChange = result.Find<FullNameDto>(FullNameXPath);
            fullNameChange.Should().NotBeNull();
            fullNameChange.NewValue.FirstName.Should().Be(firstNameNewValue);
            fullNameChange.NewValue.LastName.Should().Be(lastNameNewValue);
            fullNameChange.PreviousValue.Should().BeNull();
        }
    }
}