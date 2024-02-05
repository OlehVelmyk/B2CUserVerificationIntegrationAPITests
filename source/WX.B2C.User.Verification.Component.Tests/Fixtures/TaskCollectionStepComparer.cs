using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    /// <summary>
    /// TODO primitive logic to order steps for completion. Should be re-written later. 
    /// </summary>
    internal class TaskCollectionStepComparer : IComparer<CollectionStepVariantDto>
    {
        public int Compare(CollectionStepVariantDto x, CollectionStepVariantDto y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (ReferenceEquals(null, y))
                return 1;
            if (ReferenceEquals(null, x))
                return -1;
            
            return GetStepTypePriority(x) < GetStepTypePriority(y) ? -1 : 1;
        }

        private int GetStepTypePriority(CollectionStepVariantDto stepVariantDto) =>
            stepVariantDto switch
            {
                PersonalDetailsCollectionStepVariantDto _     => 1,
                SurveyCollectionStepVariantDto _              => 2,
                DocumentCollectionStepVariantDto _            => 3,
                VerificationDetailsCollectionStepVariantDto variant  => GetVerificationDetailsPriority(variant),
                _ => throw new ArgumentOutOfRangeException(nameof(stepVariantDto))
            };

        private int GetVerificationDetailsPriority(VerificationDetailsCollectionStepVariantDto variant) =>
            variant.Property switch
            {
                VerificationDetailsProperty.IdDocumentNumber or VerificationDetailsProperty.Nationality => 5,
                _ => 4
            };
    }
}