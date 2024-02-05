using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators;
using WX.B2C.User.Verification.Unit.Tests.Jobs.Builders;

namespace WX.B2C.User.Verification.Unit.Tests.Jobs
{
    [TestFixture]
    internal class UserConsistencyValidatorTests
    {
        private UserConsistencyValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new UserConsistencyValidator();
        }

        [Test]
        public void ShouldNotFindDefects()
        {
            var relatedTasks = new[] { Guid.Empty };

            var applicationId = Guid.NewGuid();
            var identityTaskId = Guid.NewGuid();
            var riskListsScreeningTaskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .With(Region.GB)
                .WithOnfidoApplicant(Guid.NewGuid().ToString())
                .WithPassFortProfile(Guid.NewGuid().ToString())
                .Exists().Address().Nationality().Tin().AddressDocuments().FullName().IdentityDocuments().IpAddress().RiskLevel()
                .TaxResidence().W9Form().DateOfBirth().IdDocumentNumber().IdDocumentNumberType().ProofOfFundsDocuments()
                .WithApplication(applicationId).With(ApplicationState.Approved)
                .AddTask(identityTaskId).With(TaskType.Identity).With(TaskState.Completed).WithApplicationId(applicationId)
                .AddTask(Guid.NewGuid()).With(TaskType.Address).With(TaskState.Completed).WithApplicationId(applicationId)
                .AddTask(Guid.NewGuid()).With(TaskType.DuplicationScreening).With(TaskState.Completed).WithApplicationId(applicationId)
                .AddTask(Guid.NewGuid()).With(TaskType.FinancialCondition).With(TaskState.Completed).WithApplicationId(applicationId)
                .AddTask(Guid.NewGuid()).With(TaskType.ProofOfFunds).With(TaskState.Completed).WithApplicationId(applicationId)
                .AddTask(riskListsScreeningTaskId).With(TaskType.RiskListsScreening).With(TaskState.Completed).WithApplicationId(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.Birthdate).With(CollectionStepState.Completed).AddRelatedTasks(identityTaskId)
                .AddStep(Guid.NewGuid(), XPathes.FullName).With(CollectionStepState.Completed).AddRelatedTasks(identityTaskId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber).With(CollectionStepState.Completed).AddRelatedTasks(identityTaskId)
                .AddStep(Guid.NewGuid(), XPathes.ProofOfAddressDocument).With(CollectionStepState.Completed).AddRelatedTasks(identityTaskId)
                .AddStep(Guid.NewGuid(), XPathes.ProofOfFundsDocument).With(CollectionStepState.Completed).AddRelatedTasks(identityTaskId)
                .AddStep(Guid.NewGuid(), XPathes.ProofOfIdentityDocument).With(CollectionStepState.Completed).AddRelatedTasks(identityTaskId)
                .AddStep(Guid.NewGuid(), XPathes.ResidenceAddress).With(CollectionStepState.Completed).AddRelatedTasks(identityTaskId)
                .AddStep(Guid.NewGuid(), XPathes.TaxResidence).With(CollectionStepState.Completed).AddRelatedTasks(identityTaskId)
                .AddStep(Guid.NewGuid(), XPathes.VerifiedNationality).With(CollectionStepState.Completed).AddRelatedTasks(identityTaskId)
                .AddStep(Guid.NewGuid(), XPathes.W9Form).With(CollectionStepState.Completed).AddRelatedTasks(identityTaskId)
                .AddCheck().With(CheckState.Complete).AddRelatedTasks(identityTaskId)
                .AddCheck().With(CheckState.Complete).AddRelatedTasks(riskListsScreeningTaskId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeTrue(string.Join(",", result.Errors.Select(failure => failure.ErrorCode)));
        }

        [Test]
        public void ShouldDetect_NoApplication()
        {
            var model = new UserConsistencyBuilder(Guid.NewGuid()).Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.NoApplication);
        }

        [TestCase(TaskState.Incomplete, null)]
        [TestCase(TaskState.Completed, TaskResult.Failed)]
        public void ShouldDetect_WhenApprovedWhenTaskIncompleteOrFailed(TaskState taskState, TaskResult? taskResult)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .With(ApplicationState.Approved)
                .AddTask(Guid.NewGuid())
                .With(taskState)
                .With(taskResult)
                .WithApplicationId(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.ApprovedWhenTaskIncompleteOrFailed);
        }

        [TestCase(ApplicationState.Approved, TaskState.Completed, TaskResult.Passed)]
        [TestCase(ApplicationState.InReview, TaskState.Completed, TaskResult.Failed)]
        public void ShouldNotDetect_WhenApprovedWhenTaskIncompleteOrFailed(ApplicationState applicationState, TaskState taskState, TaskResult? taskResult)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .With(applicationState)
                .AddTask(Guid.NewGuid())
                .With(taskState)
                .With(taskResult)
                .WithApplicationId(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.ApprovedWhenTaskIncompleteOrFailed);
        }

        [Test]
        public void ShouldDetect_WhenNotApprovedWhenAllTaskCompleted()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .With(ApplicationState.Applied)
                .AddTask(Guid.NewGuid())
                .With(TaskType.Identity)
                .With(TaskState.Completed)
                .AddTask(Guid.NewGuid())
                .With(TaskType.Address)
                .With(TaskState.Completed)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.NotApprovedWhenAllTaskCompleted);
        }

        [TestCase(ApplicationState.Approved, TaskState.Completed, TaskResult.Passed)]
        [TestCase(ApplicationState.Applied, TaskState.Completed, TaskResult.Failed)]
        public void ShouldNotDetect_WhenNotApprovedWhenAllTaskCompleted(ApplicationState applicationState, TaskState taskState, TaskResult? taskResult)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .With(applicationState)
                .AddTask(Guid.NewGuid())
                .With(taskState)
                .With(taskResult)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.NotApprovedWhenAllTaskCompleted);
        }

        [TestCase(ApplicationState.Approved)]
        [TestCase(ApplicationState.InReview)]
        public void ShouldDetect_WhenApprovedOrInReviewWhenNoRiskLevel(ApplicationState applicationState)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .With(applicationState)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.ApprovedOrInReviewWhenNoRiskLevel);
        }

        [Test]
        public void ShouldNotDetect_WhenApprovedOrInReviewWhenNoRiskLevel()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .With(ApplicationState.Applied)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.ApprovedOrInReviewWhenNoRiskLevel);
        }

        [Test]
        public void ShouldDetect_WhenApplicationPolicyIsNotDefined()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .With(Region.Undefined)
                .WithApplication(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.ApplicationPolicyIsNotDefined);
        }

        [Test]
        public void ShouldNotDetect_WhenApplicationPolicyIsNotDefined()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .With(Region.GB)
                .WithApplication(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.ApplicationPolicyIsNotDefined);
        }

        [Test]
        public void ShouldDetect_WhenAbsentPassFortProfile()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .With(ApplicationState.Applied)
                .AddTask(Guid.NewGuid())
                .With(TaskType.RiskListsScreening)
                .With(TaskState.Completed)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.AbsentPassFortProfile);
        }

        [Test]
        public void ShouldNotDetect_WhenAbsentPassFortProfile_ButUserFromUSA()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .With(Region.USA)
                .WithApplication(applicationId)
                .With(ApplicationState.Applied)
                .AddTask(Guid.NewGuid())
                .With(TaskType.RiskListsScreening)
                .With(TaskState.Completed)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.AbsentPassFortProfile);
        }

        [Test]
        public void ShouldNotDetect_WhenAbsentPassFortProfile_WhenTaskIncomplete_ButUserNotFromUSA()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .With(Region.GB)
                .WithApplication(applicationId)
                .With(ApplicationState.Applied)
                .AddTask(Guid.NewGuid())
                .With(TaskType.RiskListsScreening)
                .With(TaskState.Incomplete)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.AbsentPassFortProfile);
        }

        [TestCase(ApplicationState.Approved)]
        [TestCase(ApplicationState.InReview)]
        public void ShouldDetect_WhenAbsentOnfidoApplicantIdWhenApplicationWasApproved(ApplicationState applicationState)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .With(applicationState)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.AbsentOnfidoApplicantIdWhenApplicationWasApproved);
        }

        [Test]
        public void ShouldNotDetect_WhenAbsentOnfidoApplicantIdWhenApplicationWasApproved()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .With(ApplicationState.Applied)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.AbsentOnfidoApplicantIdWhenApplicationWasApproved);
        }

        [Test]
        public void ShouldDetect_WhenAbsentOnfidoApplicantIdWhenIdentityTaskCompleted()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.Identity)
                .With(TaskState.Completed)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.AbsentOnfidoApplicantIdWhenIdentityTaskCompleted);
        }

        [Test]
        public void ShouldNotDetect_WhenAbsentOnfidoApplicantIdWhenIdentityTaskCompleted()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.Identity)
                .With(TaskState.Incomplete)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.AbsentOnfidoApplicantIdWhenIdentityTaskCompleted);
        }

        [TestCase(ErrorCodes.AbsentIpAddress)]
        [TestCase(ErrorCodes.AbsentTaxResidence)]
        [TestCase(ErrorCodes.AbsentFullName)]
        [TestCase(ErrorCodes.AbsentBirthdate)]
        public void ShouldDetect_WhenProfileDataNotExist(string errorCode)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(errorCode);
        }

        [TestCase(ErrorCodes.AbsentIpAddress)]
        [TestCase(ErrorCodes.AbsentTaxResidence)]
        [TestCase(ErrorCodes.AbsentFullName)]
        [TestCase(ErrorCodes.AbsentBirthdate)]
        public void ShouldNotDetect_WhenProfileDataNotExist(string errorCode)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .Exists()
                .IpAddress()
                .TaxResidence()
                .FullName()
                .DateOfBirth()
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(errorCode);
        }

        [Test]
        public void ShouldDetect_WhenAbsentTinWhenUSA()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .With(Region.USA)
                .WithApplication(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.AbsentTinWhenUSA);
        }

        [Test]
        public void ShouldNotDetect_WhenAbsentTinWhenUSA()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .With(Region.APAC)
                .WithApplication(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.AbsentTinWhenUSA);
        }

        [Test]
        public void ShouldDetect_WhenIdDocumentTypeAbsentWhenNumberExists()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .Exists()
                .IdDocumentNumber()
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.IdDocumentTypeAbsentWhenNumberExists);
        }

        [Test]
        public void ShouldNotDetect_WhenIdDocumentTypeAbsentWhenNumberExists_WhenViceVersa()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .Exists()
                .IdDocumentNumberType()
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.IdDocumentTypeAbsentWhenNumberExists);
        }

        [Test]
        public void ShouldNotDetect_WhenIdDocumentTypeAbsentWhenNumberExists_WhenIdDocAbsent()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.IdDocumentTypeAbsentWhenNumberExists);
        }

        [Test]
        public void ShouldDetect_WhenAbsentIdDocNumberWhenIdentityTaskCompleted()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.Identity)
                .With(TaskState.Completed)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.AbsentIdDocNumberWhenIdentityTaskCompleted);
        }

        [Test]
        public void ShouldNotDetect_WhenAbsentIdDocNumberWhenIdentityTaskCompleted()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.Identity)
                .With(TaskState.Incomplete)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.AbsentIdDocNumberWhenIdentityTaskCompleted);
        }

        [Test]
        public void ShouldDetect_WhenAbsentIdentityDocumentsWhenIdentityTaskCompleted()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.Identity)
                .With(TaskState.Completed)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.AbsentIdentityDocumentsWhenIdentityTaskCompleted);
        }

        [Test]
        public void ShouldNotDetect_WhenAbsentIdentityDocumentsWhenIdentityTaskCompleted()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.Identity)
                .With(TaskState.Incomplete)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.AbsentIdentityDocumentsWhenIdentityTaskCompleted);
        }

        [Test]
        public void ShouldDetect_WhenAbsentPofDocumentsWhenPofTaskCompleted()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Completed)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.AbsentPofDocumentsWhenPofTaskCompleted);
        }

        [Test]
        public void ShouldNotDetect_WhenAbsentPofDocumentsWhenPofTaskCompleted()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Incomplete)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.AbsentPofDocumentsWhenPofTaskCompleted);
        }

        [Test]
        public void ShouldDetect_WhenTaskCompleteWhenCheckNotPerformed()
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Completed)
                .AddCheck()
                .With(CheckState.Pending)
                .AddRelatedTasks(taskId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.TaskCompleteWhenCheckNotCompleted);
        }

        [TestCase(CheckState.Complete)]
        [TestCase(CheckState.Error)]
        public void ShouldNotDetect_WhenTaskCompleteWhenCheckNotPerformed(CheckState checkState)
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Completed)
                .AddCheck()
                .With(checkState)
                .AddRelatedTasks(taskId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.TaskCompleteWhenCheckNotCompleted);
        }

        [Test]
        public void ShouldNotDetect_WhenTaskCompleteWhenCheckNotPerformed_WhenNoRelatedTasks()
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Completed)
                .AddCheck()
                .With(CheckState.Pending)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.TaskCompleteWhenCheckNotCompleted);
        }

        [Test]
        public void ShouldDetect_WhenTaskCompleteWhenCollectionStepNotComplete()
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Completed)
                .AddStep(Guid.NewGuid(), XPathes.ProofOfFundsDocument)
                .With(CollectionStepState.Requested)
                .Required()
                .AddRelatedTasks(taskId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.TaskCompleteWhenCollectionStepNotComplete);
        }

        [Test]
        public void ShouldNotDetect_WhenTaskCompleteWhenCollectionStepNotComplete()
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Completed)
                .AddStep(Guid.NewGuid(), XPathes.ProofOfFundsDocument)
                .With(CollectionStepState.Completed)
                .Required()
                .AddRelatedTasks(taskId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.TaskCompleteWhenCollectionStepNotComplete);
        }

        [Test]
        public void ShouldNotDetect_WhenTaskCompleteWhenCollectionStepNotComplete_WhenNoRelatedTask()
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Completed)
                .AddStep(Guid.NewGuid(), XPathes.ProofOfFundsDocument)
                .With(CollectionStepState.Requested)
                .Required()
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.TaskCompleteWhenCollectionStepNotComplete);
        }

        [Test]
        public void ShouldDetect_WhenTaskIncompleteWhenNoBlockers()
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Incomplete)
                .AddStep(Guid.NewGuid(), XPathes.ProofOfFundsDocument)
                .With(CollectionStepState.Completed)
                .Required()
                .AddRelatedTasks(taskId)
                .AddCheck()
                .With(CheckState.Complete)
                .AddRelatedTasks(taskId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.TaskIncompleteWhenNoBlockers);
        }

        [TestCase(TaskState.Completed, CollectionStepState.Completed, CheckState.Complete)]
        [TestCase(TaskState.Incomplete, CollectionStepState.Requested, CheckState.Pending)]
        [TestCase(TaskState.Incomplete, CollectionStepState.Completed, CheckState.Pending)]
        [TestCase(TaskState.Incomplete, CollectionStepState.Requested, CheckState.Complete)]
        public void ShouldNotDetect_WhenTaskIncompleteWhenNoBlockers(TaskState taskState, CollectionStepState stepState, CheckState checkState)
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .With(TaskType.ProofOfFunds)
                .With(taskState)
                .AddStep(Guid.NewGuid(), XPathes.ProofOfFundsDocument)
                .With(stepState)
                .Required()
                .AddRelatedTasks(taskId)
                .AddCheck()
                .With(checkState)
                .AddRelatedTasks(taskId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.TaskIncompleteWhenNoBlockers);
        }

        [Test]
        public void ShouldDetect_WhenRudimentTaskCreatedWhenNoApplicationId()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Incomplete)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.RudimentTaskCreatedWhenNoApplicationId);
        }

        [Test]
        public void ShouldNotDetect_WhenRudimentTaskCreatedWhenNoApplicationId()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Incomplete)
                .WithApplicationId(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.RudimentTaskCreatedWhenNoApplicationId);
        }

        [Test]
        public void ShouldDetect_WhenRudimentTaskCreatedWhenWrongApplicationId()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Incomplete)
                .WithApplicationId(Guid.NewGuid())
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.RudimentTaskCreatedWhenWrongApplicationId);
        }

        [Test]
        public void ShouldNotDetect_WhenRudimentTaskCreatedWhenWrongApplicationId()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Incomplete)
                .WithApplicationId(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.RudimentTaskCreatedWhenWrongApplicationId);
        }

        [Test]
        public void ShouldDetect_WhenSeveralTaskCreatedWithOneType()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Completed)
                .WithApplicationId(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Incomplete)
                .WithApplicationId(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.SeveralTaskCreatedWithOneType);
        }

        [Test]
        public void ShouldNotDetect_WhenSeveralTaskCreatedWithOneType()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.ProofOfFunds)
                .With(TaskState.Completed)
                .WithApplicationId(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.SeveralTaskCreatedWithOneType);
        }

        [Test]
        public void ShouldNotDetect_WhenSeveralTaskCreatedWithOneType_WhenNoTasks()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.SeveralTaskCreatedWithOneType);
        }

        [Test]
        public void ShouldDetect_WhenRiskListsScreeningIncompleteButNoChecks()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.RiskListsScreening)
                .With(TaskState.Incomplete)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.RiskListsScreeningIncompleteButNoChecks);
        }

        [TestCase(TaskType.RiskListsScreening, TaskState.Completed)]
        [TestCase(TaskType.Identity, TaskState.Incomplete)]
        public void ShouldNotDetect_WhenRiskListsScreeningIncompleteButNoChecks(TaskType taskType, TaskState taskState)
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .With(taskType)
                .With(taskState)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.RiskListsScreeningIncompleteButNoChecks);
        }

        [Test]
        public void ShouldNotDetect_WhenRiskListsScreeningIncompleteButNoChecks_WhenCheckPresent()
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .With(TaskType.RiskListsScreening)
                .With(TaskState.Incomplete)
                .AddCheck()
                .AddRelatedTasks(taskId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.RiskListsScreeningIncompleteButNoChecks);
        }

        [Test]
        public void ShouldDetect_WhenIdentityIncompleteButNoChecks()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.Identity)
                .With(TaskState.Incomplete)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.IdentityIncompleteButNoChecks);
        }

        [TestCase(TaskType.Identity, TaskState.Completed)]
        [TestCase(TaskType.Address, TaskState.Incomplete)]
        public void ShouldNotDetect_WhenIdentityIncompleteButNoChecks(TaskType taskType, TaskState taskState)
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .With(taskType)
                .With(taskState)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.IdentityIncompleteButNoChecks);
        }

        [Test]
        public void ShouldNotDetect_WhenIdentityIncompleteButNoChecks_WhenCheckPresent()
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .With(TaskType.Identity)
                .With(TaskState.Incomplete)
                .AddCheck()
                .AddRelatedTasks(taskId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.IdentityIncompleteButNoChecks);
        }

        [Test]
        public void ShouldDetect_WhenTaxResidenceCompletedWhenNoTaxResidence()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.TaxResidence)
                .With(TaskState.Completed)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.TaxResidenceCompletedWhenNoTaxResidence);
        }

        [TestCase(TaskType.TaxResidence, TaskState.Incomplete)]
        [TestCase(TaskType.Address, TaskState.Completed)]
        public void ShouldNotDetect_WhenTaxResidenceCompletedWhenNoTaxResidence(TaskType taskType, TaskState taskState)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(taskType)
                .With(taskState)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.TaxResidenceCompletedWhenNoTaxResidence);
        }

        [Test]
        public void ShouldNotDetect_WhenTaxResidenceCompletedWhenNoTaxResidence_TaxResidencePresent()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .Exists()
                .TaxResidence()
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .With(TaskType.TaxResidence)
                .With(TaskState.Completed)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.TaxResidenceCompletedWhenNoTaxResidence);
        }

        [TestCase(CollectionStepState.Completed)]
        [TestCase(CollectionStepState.InReview)]
        public void ShouldDetect_WhenStepInNotRequestedStateWhenDataAbsent(CollectionStepState stepState)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(stepState)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.StepInNotRequestedStateWhenDataAbsent);
        }

        [TestCase(CollectionStepState.Requested, false)]
        [TestCase(CollectionStepState.Requested, true)]
        [TestCase(CollectionStepState.Completed, true)]
        [TestCase(CollectionStepState.InReview, true)]
        public void ShouldNotDetect_WhenStepInNotRequestedStateWhenDataAbsent(CollectionStepState stepState, bool isDataExists)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .Exists()
                .IdDocumentNumber(isDataExists)
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(stepState)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.StepInNotRequestedStateWhenDataAbsent);
        }

        [Test]
        public void ShouldDetect_WhenStepRequestedWhenDataExists()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .Exists()
                .IdDocumentNumber()
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(CollectionStepState.Requested)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.StepRequestedWhenDataExists);
        }

        [TestCase(CollectionStepState.Requested, false)]
        [TestCase(CollectionStepState.Completed, true)]
        [TestCase(CollectionStepState.Completed, false)]
        [TestCase(CollectionStepState.InReview, true)]
        [TestCase(CollectionStepState.InReview, false)]
        public void ShouldNotDetect_WhenStepRequestedWhenDataExists(CollectionStepState stepState, bool isDataExists)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .Exists()
                .IdDocumentNumber(isDataExists)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(stepState)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.StepRequestedWhenDataExists);
        }

        [TestCase(CollectionStepState.Requested, CollectionStepState.Requested)]
        [TestCase(CollectionStepState.Requested, CollectionStepState.InReview)]
        [TestCase(CollectionStepState.InReview, CollectionStepState.InReview)]
        public void ShouldDetect_WhenMultipleStepsInOpenStateWithSameXPath(CollectionStepState firstStepState, CollectionStepState secondStepState)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(firstStepState)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(secondStepState)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.SeveralStepsInOpenStateWithSameXPath);
        }

        [TestCase(CollectionStepState.Completed, CollectionStepState.Completed)]
        [TestCase(CollectionStepState.Completed, CollectionStepState.Requested)]
        [TestCase(CollectionStepState.Completed, CollectionStepState.InReview)]
        public void ShouldNotDetect_WhenMultipleStepsInOpenStateWithSameXPath(CollectionStepState firstStepState, CollectionStepState secondStepState)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(firstStepState)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(secondStepState)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.SeveralStepsInOpenStateWithSameXPath);
        }

        [Test]
        public void ShouldNotDetect_WhenMultipleStepsInOpenStateWithSameXPath_WhenNoStep()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.SeveralStepsInOpenStateWithSameXPath);
        }

        [Test]
        public void ShouldNotDetect_WhenMultipleStepsInOpenStateWithSameXPath_WhenDifferentSteps()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.Birthdate)
                .With(CollectionStepState.Requested)
                .AddStep(Guid.NewGuid(), XPathes.FullName)
                .With(CollectionStepState.Requested)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.SeveralStepsInOpenStateWithSameXPath);
        }

        [Test]
        public void ShouldDetect_WhenStepCreatedButNotAttached()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(CollectionStepState.Requested)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.StepCreatedButNotAttached);
        }

        [Test]
        public void ShoulNotdDetect_WhenStepCreatedButNotAttached()
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(CollectionStepState.Requested)
                .AddRelatedTasks(taskId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.StepCreatedButNotAttached);
        }

        [Test]
        public void ShouldDetect_WhenStepInReviewWhenReviewNotRequired()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(CollectionStepState.InReview)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.StepInReviewWhenReviewNotRequired);
        }

        [Test]
        public void ShouldNotDetect_WhenStepInReviewWhenReviewNotRequired()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(CollectionStepState.InReview)
                .ReviewRequired()
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.StepInReviewWhenReviewNotRequired);
        }

        [Test]
        public void ShouldDetect_WhenAbsentReviewResultWhenCompletedStepRequiresReview()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(CollectionStepState.Completed)
                .ReviewRequired()
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.AbsentReviewResultWhenCompletedStepRequiresReview);
        }

        [TestCase(CollectionStepState.Completed, true, CollectionStepReviewResult.Approved)]
        [TestCase(CollectionStepState.Requested, true, null)]
        [TestCase(CollectionStepState.Completed, false, null)]
        public void ShouldNotDetect_WhenAbsentReviewResultWhenCompletedStepRequiresReview(CollectionStepState stepState, 
                                                                                          bool isReviewRequired, 
                                                                                          CollectionStepReviewResult? reviewResult)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(stepState)
                .With(reviewResult)
                .ReviewRequired(isReviewRequired)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.AbsentReviewResultWhenCompletedStepRequiresReview);
        }

        [TestCase(CollectionStepState.Requested, false)]
        [TestCase(CollectionStepState.InReview, false)]
        [TestCase(CollectionStepState.Completed, false)]
        [TestCase(CollectionStepState.Requested, true)]
        [TestCase(CollectionStepState.InReview, true)]
        public void ShouldDetect_WhenRudimentReviewResult(CollectionStepState stepState, bool isReviewRequired)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(stepState)
                .With(CollectionStepReviewResult.Approved)
                .ReviewRequired(isReviewRequired)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.RudimentReviewResult);
        }

        [TestCase(CollectionStepState.Completed, CollectionStepReviewResult.Approved, true)]
        [TestCase(CollectionStepState.Requested, null, true)]
        [TestCase(CollectionStepState.Completed, null, false)]
        [TestCase(CollectionStepState.InReview, null, false)]
        [TestCase(CollectionStepState.Requested, null, false)]
        public void ShouldNotDetect_WhenRudimentReviewResult(CollectionStepState stepState, CollectionStepReviewResult? reviewResult, bool isReviewRequired)
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(stepState)
                .With(reviewResult)
                .ReviewRequired(isReviewRequired)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.RudimentReviewResult);
        }

        [Test]
        public void ShouldDetect_WhenStepsMustBeAttachedOnlyToApplicationTasks()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(Guid.NewGuid())
                .WithApplicationId(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(CollectionStepState.Requested)
                .AddRelatedTasks(Guid.NewGuid())
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.StepsMustBeAttachedOnlyToApplicationTasks);
        }

        [Test]
        public void ShouldDetect_WhenStepsMustBeAttachedOnlyToApplicationTasks_WhenNoTasks()
        {
            var applicationId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(CollectionStepState.Requested)
                .AddRelatedTasks(Guid.NewGuid())
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.StepsMustBeAttachedOnlyToApplicationTasks);
        }

        [Test]
        public void ShouldDetect_WhenStepsMustBeAttachedOnlyToApplicationTasks_WhenStepInNotApplicationTask()
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(CollectionStepState.Requested)
                .AddRelatedTasks(taskId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldContain(ErrorCodes.StepsMustBeAttachedOnlyToApplicationTasks);
        }

        [Test]
        public void ShouldNotDetect_WhenStepsMustBeAttachedOnlyToApplicationTasks()
        {
            var applicationId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var model = new UserConsistencyBuilder(Guid.NewGuid())
                .WithApplication(applicationId)
                .AddTask(taskId)
                .WithApplicationId(applicationId)
                .AddStep(Guid.NewGuid(), XPathes.IdDocumentNumber)
                .With(CollectionStepState.Requested)
                .AddRelatedTasks(taskId)
                .Build();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeFalse();
            result.ShouldNotContain(ErrorCodes.StepsMustBeAttachedOnlyToApplicationTasks);
        }
    }

    internal static class ValidationResultExtensions
    {
        public static void ShouldContain(this ValidationResult result, string errorCode) =>
            result.Errors.Select(failure => failure.ErrorCode).Distinct().Should().Contain(errorCode);

        public static void ShouldNotContain(this ValidationResult result, string errorCode) =>
            result.Errors.Select(failure => failure.ErrorCode).Distinct().Should().NotContain(errorCode);
    }
}