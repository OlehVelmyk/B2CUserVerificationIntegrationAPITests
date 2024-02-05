using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Enums;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    [KnownType(typeof(SurveyCollectionStepVariantDto))]
    [KnownType(typeof(DocumentCollectionStepVariantDto))]
    [KnownType(typeof(PersonalDetailsCollectionStepVariantDto))]
    [KnownType(typeof(VerificationDetailsCollectionStepVariantDto))]
    public abstract class CollectionStepVariantDto
    {
        public string Name { get; set; }

        public abstract CollectionStepType Type { get; }
    }

    public sealed class PersonalDetailsCollectionStepVariantDto : CollectionStepVariantDto
    {
        public override CollectionStepType Type => CollectionStepType.PersonalDetails;

        public PersonalDetailsProperty Property { get; set; }
    }

    public sealed class VerificationDetailsCollectionStepVariantDto : CollectionStepVariantDto
    {
        public override CollectionStepType Type => CollectionStepType.VerificationDetails;

        public VerificationDetailsProperty Property { get; set; }
    }

    public sealed class SurveyCollectionStepVariantDto : CollectionStepVariantDto
    {
        public override CollectionStepType Type => CollectionStepType.Survey;

        public Guid TemplateId { get; set; }
    }

    public sealed class DocumentCollectionStepVariantDto : CollectionStepVariantDto
    {
        public override CollectionStepType Type => CollectionStepType.Document;

        public DocumentCategory DocumentCategory { get; set; }

        public string DocumentType { get; set; }
    }
}