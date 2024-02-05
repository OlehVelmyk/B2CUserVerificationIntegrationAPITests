using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Enums;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    [KnownType(typeof(PersonalDetailsCollectionStepRequest))]
    [KnownType(typeof(VerificationDetailsCollectionStepRequest))]
    [KnownType(typeof(DocumentCollectionStepRequest))]
    [KnownType(typeof(SurveyCollectionStepRequest))]
    public abstract class CollectionStepRequest
    {
        public CollectionStepType Type { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }

        public Guid[] TargetTasks { get; set; }

        public string Reason { get; set; }
    }

    public sealed class VerificationDetailsCollectionStepRequest : CollectionStepRequest
    {
        public VerificationDetailsProperty VerificationProperty { get; set; }
    }

    public sealed class PersonalDetailsCollectionStepRequest : CollectionStepRequest
    {
        public PersonalDetailsProperty PersonalProperty { get; set; }
    }

    public sealed class DocumentCollectionStepRequest : CollectionStepRequest
    {
        public DocumentCategory DocumentCategory { get; set; }

        [NotRequired]
        public string DocumentType { get; set; }
    }

    public sealed class SurveyCollectionStepRequest : CollectionStepRequest
    {
        public Guid TemplateId { get; set; }
    }
}