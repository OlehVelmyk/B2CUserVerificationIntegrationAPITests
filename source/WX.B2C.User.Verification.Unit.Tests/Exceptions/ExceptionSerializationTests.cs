using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Exceptions;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Unit.Tests.Extensions;

namespace WX.B2C.User.Verification.Unit.Tests.Exceptions
{
    [TestFixture]
    internal class ExceptionSerializationTests
    {
        [Test]
        public void ExceptionInDomain_ShouldBeInInheritedFromDomainException()
        {
            var assembly = typeof(B2CVerificationException).Assembly;
            var exceptions = assembly.GetTypes().Where(type => typeof(Exception).IsAssignableFrom(type));

            exceptions.Should().AllTypesBeAssignableTo<B2CVerificationException>();
        }

        [Test]
        public void DomainException_ShouldBeInTestSource()
        {
            var baseType = typeof(B2CVerificationException);
            var assembly = baseType.Assembly;
            var expected = assembly.GetTypes().Where(type => baseType.IsAssignableFrom(type) && type != baseType && !type.IsAbstract);
            var actual = DomainExceptionSource().Select(exception => exception.GetType());

            expected.Except(actual).Should().BeEmpty();
        }

        [Test]
        public void ExceptionInContracts_ShouldBeInInheritedFromDomainException()
        {
            var assembly = typeof(BlobStorageFileNotFoundException).Assembly;
            var exceptions = assembly.GetTypes().Where(type => typeof(Exception).IsAssignableFrom(type));

            exceptions.Should().AllTypesBeAssignableTo<B2CVerificationException>();
        }

        [Test]
        public void ContractsException_ShouldBeInTestSource()
        {
            var assembly = typeof(BlobStorageFileNotFoundException).Assembly;
            var expected = assembly.GetTypes().Where(type => typeof(B2CVerificationException).IsAssignableFrom(type));
            var actual = DomainExceptionSource().Select(exception => exception.GetType());

            expected.Except(actual).Should().BeEmpty();
        }

        [Test]
        [TestCaseSource(nameof(DomainExceptionSource))]
        public void ShouldBeSerializable(Exception exception)
        {
            exception.Should().BeDataContractSerializable();
        }

        private static IEnumerable<B2CVerificationException> DomainExceptionSource()
        {
            var testGuid = Guid.NewGuid();
            var testGuidString = testGuid.ToString();
            var testString = "test";

            yield return new DatabaseException(new ArgumentNullException());

            yield return new DatabaseConcurrencyException(new ArgumentNullException());

            yield return new EntityNotFoundException(testGuidString, testGuid);

            yield return new ApproveApplicationException(testGuid, Enumerable.Repeat(testGuid, 3));

            yield return new BlobStorageFileNotFoundException(testString, testString);

            yield return new BridgerPasswordUpdateException(testString);

            yield return new CheckInputValidationException(Enumerable.Repeat(testString, 3).ToArray());

            yield return new InconsistentRevertDecisionOperationException(testGuid);

            yield return InvalidStateTransitionException.For<Application>(ApplicationState.Approved, ApplicationState.Cancelled);

            yield return new TaskAlreadyCompletedException(testGuid, TaskResult.Passed);

            yield return new TaskStepsNotCompletedException(testGuid, new[] { testGuid, testGuid });

            yield return new TaskChecksNotCompletedException(testGuid, new[] { testGuid, testGuid });

            yield return new CollectionStepAlreadyCompletedException(testGuid, testString);

            yield return new CollectionStepAlreadyCompletedException(testGuid, testString);

            yield return new CollectionStepReviewProhibitedException(testGuid, testString);

            yield return new TaskExpiredException(testGuid, testGuid);

            yield return new CollectionStepReviewRequiredException(testGuid, testString);

            yield return new CheckAlreadyCompletedException(testGuid, CheckState.Complete);

            yield return new CheckInputValidationException(testString);

            yield return new CheckExecutionException(testString, testString);

            yield return new CheckProcessingException(testString, testString);

            yield return new ApplicationTaskAlreadyExistsException(
                VerificationTask.Create(
                    testGuid,
                    testGuid,
                    TaskType.Address,
                    testGuid,
                    DateTime.UtcNow,
                    Array.Empty<CollectionStep>(),
                    new Initiation(testString, testString)),
                testGuid);

            var collectionStep = new CollectionStep(testGuid, testGuid, testString, CollectionStepState.Completed, true, true);
            var verificationTask = VerificationTask.Create(
            testGuid,
            testGuid,
            TaskType.Address,
            testGuid,
            DateTime.UtcNow,
            Array.Empty<CollectionStep>(),
            new Initiation(testString, testString));

            yield return new UserMismatchedException(verificationTask, collectionStep);

            yield return new ExternalFileNotFoundException(testGuidString, testGuidString, ExternalFileProviderType.Onfido);
        }
    }
}
