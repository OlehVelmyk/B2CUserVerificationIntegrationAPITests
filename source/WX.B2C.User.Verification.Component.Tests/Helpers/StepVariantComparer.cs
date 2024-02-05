using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Helpers
{
    internal class StepVariantComparer : IEqualityComparer<CollectionStepVariantDto>
    {
        public bool Equals([AllowNull] CollectionStepVariantDto x, [AllowNull] CollectionStepVariantDto y)
        {
            if(ReferenceEquals(x, y))
                return true;
            if (x is null && y is null)
                return true;
            if (x is null || y is null)
                return false;
            if (x.GetType() != y.GetType())
                return false;

            if (x is PersonalDetailsCollectionStepVariantDto px && y is PersonalDetailsCollectionStepVariantDto py)
                return px.Property == py.Property;
            if (x is VerificationDetailsCollectionStepVariantDto vx && y is VerificationDetailsCollectionStepVariantDto vy)
                return vx.Property == vy.Property;
            if (x is DocumentCollectionStepVariantDto dx && y is DocumentCollectionStepVariantDto dy)
                return dx.DocumentCategory == dy.DocumentCategory && dx.DocumentType == dy.DocumentType;
            if (x is SurveyCollectionStepVariantDto sx && y is SurveyCollectionStepVariantDto sy)
                return sx.TemplateId == sy.TemplateId;

            throw new ArgumentOutOfRangeException();
        }

        public int GetHashCode([DisallowNull] CollectionStepVariantDto obj) =>
            obj.Name.GetHashCode();
    }
}
