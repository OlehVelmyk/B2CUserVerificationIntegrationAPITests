using System;
using FluentAssertions;
using NUnit.Framework;
using Optional;
using WX.B2C.User.Verification.Core.Services.Extensions;

namespace WX.B2C.User.Verification.Unit.Tests.Services
{
    internal class PatchExtensionsTests
    {
        private class PatchTestsDummy
        {
            public string Property { get; set; }

            public string OnlyReadableProperty { get; }

            public string Field;
        }

        private class PatchTestsDummyArray
        {
            public PatchTestsDummy[] Dummies { get; set; }
        }

        [Test]
        public void ShouldThrowArgumentNullException()
        {
            PatchTestsDummy nullableDummy = null;
            var dummy = new PatchTestsDummy();

            Func<PatchResult> modelNullExceptionFunc = () =>
                nullableDummy.Patch(
                    model => model.Property,
                    Option.None<string>);
            Func<PatchResult> propertySelectorNullExceptionFunc = () =>
                dummy.Patch(
                    null,
                    Option.None<string>);
            Func<PatchResult> newValueProviderPropertyNullExceptionFunc = () =>
                dummy.Patch(
                model => model.OnlyReadableProperty, 
                null);

            modelNullExceptionFunc.Should().Throw<ArgumentNullException>();
            propertySelectorNullExceptionFunc.Should().Throw<ArgumentNullException>();
            newValueProviderPropertyNullExceptionFunc.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ShouldThrowInvalidOperationException()
        {
            var dummy = new PatchTestsDummy();

            Func<PatchResult> notPropertyExceptionFunc = () =>
                dummy.Patch(
                    model => string.Empty,
                    Option.None<string>);
            Func<PatchResult> fieldNotPropertyExceptionFunc = () =>
                dummy.Patch(
                    model => model.Field,
                    Option.None<string>);
            Func<PatchResult> notWritablePropertyExceptionFunc = () =>
                dummy.Patch(
                    model => model.OnlyReadableProperty,
                    Option.None<string>);

            notPropertyExceptionFunc.Should().Throw<InvalidOperationException>();
            fieldNotPropertyExceptionFunc.Should().Throw<InvalidOperationException>();
            notWritablePropertyExceptionFunc.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void ShouldUpdate()
        {
            const string FirstValue = "test1";
            const string SecondValue = "test2";
            var dummy = new PatchTestsDummy();

            var patchResult = dummy.Patch(
                model => model.Property,
                () => FirstValue.Some());
            patchResult.IsPatched.Should().BeTrue();
            patchResult.OldValue.Should().BeNull();
            patchResult.NewValue.Should().Be(FirstValue);
            dummy.Property.Should().Be(FirstValue);

            patchResult = dummy.Patch(
                model => model.Property,
                () => SecondValue.Some());
            patchResult.IsPatched.Should().BeTrue();
            patchResult.OldValue.Should().Be(FirstValue);
            patchResult.NewValue.Should().Be(SecondValue);
            dummy.Property.Should().Be(SecondValue);
        }

        [Test]
        public void ShouldNotUpdate()
        {
            const string FirstValue = "test1";
            const string SecondValue = "test2";
            var dummy = new PatchTestsDummy();

            var patchResult = dummy.Patch(
                model => model.Property,
                () => FirstValue.Some(), _ => true);
            patchResult.IsPatched.Should().BeFalse();
            patchResult.NewValue.Should().Be(patchResult.OldValue).And.BeNull();
            dummy.Property.Should().BeNull();

            patchResult = dummy.Patch(
                model => model.Property,
                () => SecondValue.Some(), _ => false,
                (_, _) => true);
            patchResult.NewValue.Should().Be(patchResult.OldValue).And.BeNull();
            patchResult.IsPatched.Should().BeFalse();
            dummy.Property.Should().BeNull();
        }

        [Test]
        public void ShouldUpdateArray()
        {
            var dummyArray = new PatchTestsDummyArray();
            var dummies = new PatchTestsDummy[3];

            var patchResult = dummyArray.Patch(
                model => model.Dummies, 
                () => dummies.Some());

            patchResult.IsPatched.Should().BeTrue();
            patchResult.OldValue.Should().BeNull();
            patchResult.NewValue.Should().BeEquivalentTo(dummies);
            dummyArray.Dummies.Should().HaveSameCount(dummies)
                      .And.BeEquivalentTo(dummies);
        }

        [Test]
        public void ShouldNotUpdateArray()
        {
            var dummyArray = new PatchTestsDummyArray();

            var patchResult = dummyArray.Patch(
                model => model.Dummies, 
                Option.None<PatchTestsDummy[]>);
            patchResult.IsPatched.Should().BeFalse();
            patchResult.NewValue.Should().BeEquivalentTo(patchResult.OldValue).And.BeNull();
            dummyArray.Dummies.Should().BeNull();

            patchResult = dummyArray.Patch(
                model => model.Dummies,
                () => Array.Empty<PatchTestsDummy>().Some(),
                dummies => dummies.Length == 0);
            patchResult.IsPatched.Should().BeFalse();
            patchResult.NewValue.Should().BeEquivalentTo(patchResult.OldValue).And.BeNull();
            dummyArray.Dummies.Should().BeNull();
        }
    }
}
