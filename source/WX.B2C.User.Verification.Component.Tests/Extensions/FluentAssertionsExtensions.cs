using System;
using System.IO;
using FluentAssertions;
using FluentAssertions.Streams;
using System.Collections.Generic;
using FluentAssertions.Collections;
using FluentAssertions.Primitives;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static class FluentAssertionsExtensions
    {
        public static AndConstraint<TAssertions> BeSomeOrNull<TEnum, TAssertions>(
            this NullableEnumAssertions<TEnum, TAssertions> assertions, Predicate<TEnum?> someOrNull, string because = "", params object[] becauseArgs)
            where TEnum : struct, Enum
            where TAssertions : NullableEnumAssertions<TEnum, TAssertions>
        {
            if (assertions == null)
                throw new ArgumentNullException(nameof(assertions));
            if (someOrNull == null)
                throw new ArgumentNullException(nameof(someOrNull));

            return assertions.Match(subject => someOrNull(subject) ? subject != null : subject == null, because, becauseArgs);
        }

        public static AndConstraint<TAssertions> BeSomeOrNull<TSubject, TAssertions>(
            this ReferenceTypeAssertions<TSubject, TAssertions> assertions, Predicate<TSubject> someOrNull, string because = "", params object[] becauseArgs)
            where TAssertions : ReferenceTypeAssertions<TSubject, TAssertions>
        {
            if (assertions == null)
                throw new ArgumentNullException(nameof(assertions));
            if (someOrNull == null)
                throw new ArgumentNullException(nameof(someOrNull));

            return assertions.Match(subject => someOrNull(subject) ? subject != null : subject == null, because, becauseArgs);
        }

        public static void BeEquivalentTo(this StreamAssertions assertions, Stream expected)
        {
            if (assertions is null)
                throw new ArgumentNullException(nameof(assertions));

            var actual = assertions.Subject;
            if (actual == expected)
                return;

            actual.Should().HaveLength(expected.Length);
            actual.Should().HavePosition(expected.Position);

            const int EndOfTheStream = -1;
            while (true)
            {
                var actualValue = actual.ReadByte();
                var expectedValue = expected.ReadByte();
                actualValue.Should().Be(expectedValue);

                if (actualValue == EndOfTheStream)
                    break;
            }
        }

        public static AndConstraint<EnumAssertions<TEnum>> HaveValue<TEnum>(this EnumAssertions<TEnum> assertions) where TEnum : struct, Enum
        {
            if (assertions is null)
                throw new ArgumentNullException(nameof(assertions));
            if (assertions.Subject is null)
                throw new ArgumentNullException(nameof(assertions.Subject));

            var @enum = assertions.Subject.ToString();
            Enum.GetNames(typeof(TEnum)).Should().Contain(@enum, $"Value {@enum} is not found in enum {typeof(TEnum).Name}");

            return new AndConstraint<EnumAssertions<TEnum>>(assertions);
        }

        public static AndWhichConstraint<GenericCollectionAssertions<UserActionDto>, UserActionDto> ContainsSurvey(
            this GenericCollectionAssertions<UserActionDto> actionsAssertions,
            string surveyId,
            string surveyTag)
        {
            if (actionsAssertions is null) throw new ArgumentNullException(nameof(actionsAssertions));
            if (surveyId is null) throw new ArgumentNullException(nameof(surveyId));
            if (surveyTag is null) throw new ArgumentNullException(nameof(surveyTag));

            return actionsAssertions.Contain(action => action.ActionType == ActionType.Survey)
                                    .And.Contain(action => action.ActionData.ContainsSurvey(surveyId, surveyTag));
        }

        public static AndConstraint<NullableEnumAssertions<TEnum>> HaveValue<TEnum>(this NullableEnumAssertions<TEnum> assertions) where TEnum : struct, Enum
        {
            if (assertions is null)
                throw new ArgumentNullException(nameof(assertions));

            var @enum = assertions.Subject.Should().NotBeNull().And.Subject.ToString();
            Enum.GetNames(typeof(TEnum)).Should().Contain(@enum, $"Value {@enum} is not found in enum {typeof(TEnum).Name}");

            return new AndConstraint<NullableEnumAssertions<TEnum>>(assertions);
        }

        public static AndWhichConstraint<GenericCollectionAssertions<PropertyChangeDto>, PropertyChange<T>> Contain<T>(
            this GenericCollectionAssertions<PropertyChangeDto> assertions,
            string key)
        {
            var change = assertions.Subject.Find<T>(key);
            change.Should().NotBeNull();
            return new AndWhichConstraint<GenericCollectionAssertions<PropertyChangeDto>, PropertyChange<T>>(assertions, change);
        }

        public static PropertyChangeAssertions<T> Should<T>(this PropertyChange<T> change)
        {
            return new PropertyChangeAssertions<T>(change);
        }

        public static AndConstraint<PropertyChangeAssertions<T>> Match<T>(this PropertyChangeAssertions<T> assertions,
                                                                          Action<T> oldAction,
                                                                          Action<T> newAction)
        {
            var change = assertions.Subject;

            oldAction?.Invoke(change.PreviousValue);
            newAction?.Invoke(change.NewValue);

            return new AndConstraint<PropertyChangeAssertions<T>>(assertions);
        }

        private static bool ContainsSurvey(this IDictionary<string, string> dictionary, string surveyId, string surveyTag) =>
            (dictionary?.Contains(KeyValuePair.Create(ActionDataKeys.SurveyId, surveyId)) ?? false) &&
            dictionary.Contains(KeyValuePair.Create(ActionDataKeys.SurveyTag, surveyTag));
    }

    internal class PropertyChangeAssertions<T> : ObjectAssertions<PropertyChange<T>, PropertyChangeAssertions<T>>
    {
        public PropertyChangeAssertions(PropertyChange<T> subject)
            : base(subject) { }
    }
}
