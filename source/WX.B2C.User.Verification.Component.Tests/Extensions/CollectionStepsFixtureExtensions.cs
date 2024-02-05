using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Fixtures;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static class CollectionStepsFixtureExtensions
    {
        public static Task<Guid> RequestAsync(this CollectionStepsFixture fixture, Guid userId, PersonalDetailsProperty property, Guid[] targetTasks = null)
        {
            var request = new PersonalDetailsCollectionStepRequest
            {
                Reason = nameof(CollectionStepsFixtureExtensions),
                Type = CollectionStepType.PersonalDetails,
                IsRequired = true,
                IsReviewNeeded = false,
                PersonalProperty = property,
                TargetTasks = targetTasks
            };

            return fixture.RequestAsync(userId, request);
        }

        public static Task<Guid> RequestAsync(this CollectionStepsFixture fixture, Guid userId, VerificationDetailsProperty property, Guid[] targetTasks = null)
        {
            var request = new VerificationDetailsCollectionStepRequest
            {
                Reason = nameof(CollectionStepsFixtureExtensions),
                Type = CollectionStepType.VerificationDetails,
                IsRequired = true,
                IsReviewNeeded = false,
                VerificationProperty = property,
                TargetTasks = targetTasks
            };

            return fixture.RequestAsync(userId, request);
        }

        public static Task<Guid> RequestAsync(this CollectionStepsFixture fixture, Guid userId, DocumentCategory category, string type = null, Guid[] targetTasks = null)
        {
            var request = new DocumentCollectionStepRequest
            {
                Reason = nameof(CollectionStepsFixtureExtensions),
                Type = CollectionStepType.Document,
                IsRequired = true,
                IsReviewNeeded = false,
                DocumentCategory = category,
                DocumentType = type,
                TargetTasks = targetTasks
            };

            return fixture.RequestAsync(userId, request);
        }

        public static Task<Guid> RequestAsync(this CollectionStepsFixture fixture, Guid userId, string templateId, Guid[] targetTasks = null) =>
            RequestAsync(fixture, userId, new Guid(templateId), targetTasks);

        public static Task<Guid> RequestAsync(this CollectionStepsFixture fixture, Guid userId, Guid templateId, Guid[] targetTasks = null)
        {
            var request = new SurveyCollectionStepRequest
            {
                Reason = nameof(CollectionStepsFixtureExtensions),
                Type = CollectionStepType.Survey,
                IsRequired = true,
                IsReviewNeeded = false,
                TemplateId = templateId,
                TargetTasks = targetTasks
            };

            return fixture.RequestAsync(userId, request);
        }
    }
}
