using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;

namespace WX.B2C.User.Verification.Unit.Tests.Extensions
{
    internal static class FluentAssertionExtensions
    {
        /// <summary>
        /// Asserts that all items in the collection are of the specified type <typeparamref name="TExpectation" />
        /// </summary>
        /// <typeparam name="TExpectation">The expected type of the objects</typeparam>
        /// <param name="collectionAssertions">Collection if assertion types</param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <paramref name="because" />.
        /// </param>
        public static AndWhichConstraint<GenericCollectionAssertions<Type>, IEnumerable<TExpectation>> AllTypesBeAssignableTo<TExpectation>(
            this GenericCollectionAssertions<Type> collectionAssertions,
            string because = "",
            params object[] becauseArgs)
        {
            bool success = Execute.Assertion
                                  .BecauseOf(because, becauseArgs)
                                  .ForCondition(collectionAssertions.Subject is not null)
                                  .FailWith("Expected type to be {0}{reason}, but found {context:the collection} is <null>.",
                                            typeof(TExpectation).FullName);

            IEnumerable<TExpectation> matches = Array.Empty<TExpectation>();

            if (success)
            {
                Execute.Assertion
                       .BecauseOf(because, becauseArgs)
                       .WithExpectation("Expected type to be {0}{reason}, ", typeof(TExpectation).FullName)
                       .ForCondition(collectionAssertions.Subject.All(x => x is not null))
                       .FailWith("but found a null element.")
                       .Then
                       .ForCondition(collectionAssertions.Subject.All(IsAssignable))
                       .FailWith("but found {0}.", () => $"[{string.Join(", ", collectionAssertions.Subject.Where(IsNotAssignable).Select(x => x.FullName))}]")
                       .Then
                       .ClearExpectation();

                matches = collectionAssertions.Subject.OfType<TExpectation>();
            }

            return new AndWhichConstraint<GenericCollectionAssertions<Type>, IEnumerable<TExpectation>>(collectionAssertions, matches);

            bool IsAssignable(Type type) =>
                typeof(TExpectation).IsAssignableFrom(type);

            bool IsNotAssignable(Type type) =>
                !typeof(TExpectation).IsAssignableFrom(type);
        }
    }
}