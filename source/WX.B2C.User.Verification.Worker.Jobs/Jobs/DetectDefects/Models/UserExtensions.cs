using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Models;
using DBModel = WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.ProviderModels;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models
{
    internal static class UserExtensions
    {
        private static Dictionary<Guid, Region> HardcodedRegions = new()
        {
            { Guid.Parse("0EAAE368-8ACB-410B-8EC0-3AE404F49D5E"), Region.EEA },
            { Guid.Parse("DC658B4F-A0EB-4C20-B296-E0D57E8DA6DB"), Region.GB },
            { Guid.Parse("37C6AD01-067C-4B80-976D-30A568E7B0CD"), Region.APAC },
            { Guid.Parse("4B6271BD-FDE5-40F7-8701-29AA66865568"), Region.USA },
            { Guid.Parse("D5B5997E-FFC1-495D-9E98-60CCBDD6F43B"), Region.Raw },
            { Guid.Parse("5DECE2A9-CDD3-4D0D-B1BC-8A164B745051"), Region.Global },
            { Guid.Parse("67A2B2C8-BEAB-4C3E-A772-19CE9380CB0E"), Region.RU },
        };

        public static UserConsistency With(this UserConsistency user, DBModel.Profile profile)
        {
            if (profile == null)
                return user;

            user.Application = new Application
            {
                Id = profile.ApplicationId,
                State = profile.ApplicationState
            };

            user.ProfileDataExistence = new ProfileDataExistence
            {
                Address = profile.Address,
                Nationality = profile.Nationality,
                Tin = profile.Tin,
                FullName = profile.FullName,
                IpAddress = profile.IpAddress,
                RiskLevel = profile.RiskLevel,
                TaxResidence = profile.TaxResidence,
                DateOfBirth = profile.DateOfBirth,
                IdDocumentNumber = profile.IdDocumentNumber,
                IdDocumentNumberType = profile.IdDocumentNumberType
            };

            if (HardcodedRegions.TryGetValue(profile.PolicyId, out var userRegion))
                user.Region = userRegion;
            
            return user;
        }

        public static UserConsistency With(this UserConsistency user, IEnumerable<DBModel.ExternalProfile> externalProfiles)
        {
            user.PassFortProfileId =
                externalProfiles.FirstOrDefault(profile => profile.Provider == ExternalProviderType.PassFort)?.ExternalId;
            
            user.OnfidoApplicationId =
                externalProfiles.FirstOrDefault(profile => profile.Provider == ExternalProviderType.Onfido)?.ExternalId;

            return user;
        }

        public static UserConsistency With(this UserConsistency user, IEnumerable<DBModel.Document> documents)
        {
            var existingCategories = documents.Select(document => document.Category).Distinct();
            foreach (var existingCategory in existingCategories)
            {
                switch (existingCategory)
                {
                    case DocumentCategory.ProofOfIdentity:
                        user.ProfileDataExistence.IdentityDocuments = true;
                        break;
                    case DocumentCategory.ProofOfAddress:
                        user.ProfileDataExistence.AddressDocuments = true;
                        break;
                    case DocumentCategory.Taxation:
                        user.ProfileDataExistence.W9Form = true;
                        break;
                    case DocumentCategory.ProofOfFunds:
                        user.ProfileDataExistence.ProofOfFundsDocuments = true;
                        break;
                }
            }
            
            return user;
        }

        public static UserConsistency With(this UserConsistency user, IEnumerable<DBModel.Task> tasks)
        {
            user.Tasks = tasks.Select(task => new Task
            {
                Type = task.Type,
                Id = task.Id,
                Result = task.Result,
                State = task.State,
                ApplicationId = task.ApplicationId
            }).ToArray();
            return user;
        }

        public static UserConsistency With(this UserConsistency user, IEnumerable<DBModel.Check> checks)
        {
            user.Checks = checks.GroupBy(check => check.Id)
                                .Select(grouping => new Check
                                {
                                    Id = grouping.First().Id,
                                    State = grouping.First().State,
                                    RelatedTasks = grouping.Where(check => check.TaskId.HasValue)
                                                           .Select(check => check.TaskId.Value)
                                                           .ToArray()
                                })
                                .ToArray();
            return user;
        }        
        
        public static UserConsistency With(this UserConsistency user, IEnumerable<DBModel.CollectionStep> steps)
        {
            user.CollectionSteps = steps.GroupBy(step => step.Id).Select(grouping =>
            {
                var step = grouping.First();
                var relatedTasks = grouping.Where(collectionStep => collectionStep.TaskId.HasValue)
                                           .Select(collectionStep => collectionStep.TaskId.Value)
                                           .ToArray();

                return new CollectionStep
                {
                    Id = step.Id,
                    Result = step.ReviewResult,
                    State = step.State,
                    CreatedAt = step.CreatedAt,
                    IsRequired = step.IsRequired,
                    XPath = step.XPath,
                    IsReviewRequired = step.IsReviewNeeded,
                    RelatedTasks = relatedTasks
                };
            }).ToArray();
            return user;
        }
    }
}