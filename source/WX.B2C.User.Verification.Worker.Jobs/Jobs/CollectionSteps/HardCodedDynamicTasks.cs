using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    /// <summary>
    /// Provide dynamic collection steps in tasks (steps which can be added to tasks in the process of verification)
    /// </summary>
    internal static class HardCodedDynamicTasks
    {
        private static TaskVariantDto[] EeaTasks = 
        {
            new()
            {
                Type = TaskType.Address,
                VariantId = Guid.Parse("A8D1CA2B-1ABB-49D7-A5AE-74E5DD3D9F5E"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.ProofOfAddressDocument,
                        IsRequired = false,
                        IsReviewNeeded = true
                    }
                }
            },            
            new()
            {
                Type = TaskType.ProofOfFunds,
                VariantId = Guid.Parse("9E202236-42B4-4105-A6A4-1356A82911A2"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.ProofOfFundsDocument,
                        IsRequired = false,
                        IsReviewNeeded = true
                    },
                    new PolicyCollectionStep
                    {
                        XPath = "Survey.0FB7492B-7DC5-4277-A7FF-F3D07376FF66",
                        IsRequired = false,
                        IsReviewNeeded = true
                    },
                }
            },
            new()
            {
                Type = TaskType.TaxResidence,
                VariantId = Guid.Parse("2F2E1614-B199-4D7A-A8F0-E9AA810E29EC"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.W9Form,
                        IsRequired = true,
                        IsReviewNeeded = true
                    }
                }
            },
            new()
            {
                Type = TaskType.FinancialCondition,
                VariantId = Guid.Parse("41F23381-F8BB-42C7-8B58-C344A4AD011E"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = "Survey.F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686",
                        IsRequired = false,
                        IsReviewNeeded = false
                    }
                }
            },
        };

        private static TaskVariantDto[] GbTasks =
        {
            new()
            {
                Type = TaskType.ProofOfFunds,
                VariantId = Guid.Parse("C1B826B7-6E0A-4AB3-813D-4393E0C0E095"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.ProofOfFundsDocument,
                        IsRequired = true,
                        IsReviewNeeded = true
                    },                    
                    new PolicyCollectionStep
                    {
                        XPath = "Survey.0FB7492B-7DC5-4277-A7FF-F3D07376FF66",
                        IsRequired = true,
                        IsReviewNeeded = false
                    },
                }
            },
            new()
            {
                Type = TaskType.FinancialCondition,
                VariantId = Guid.Parse("CF9F0CA8-6535-4F4A-B2D3-71AB075EC841"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = "Survey.F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686",
                        IsRequired = true,
                        IsReviewNeeded = false
                    }
                }
            },
            new()
            {
                Type = TaskType.Address,
                VariantId = Guid.Parse("543EC1A7-6141-4491-9CA6-3691BBDDB7EE"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.ProofOfAddressDocument,
                        IsRequired = true,
                        IsReviewNeeded = true
                    }
                }
            },
            new()
            {
                Type = TaskType.TaxResidence,
                VariantId = Guid.Parse("F8743F21-A511-45A3-AB03-E833AB639AFA"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.W9Form,
                        IsRequired = true,
                        IsReviewNeeded = true
                    }
                }
            },
            new()
            {
                Type = TaskType.RiskListsScreening,
                VariantId = Guid.Parse("9A1C60FB-6F96-431B-ABDB-11B5FC9C5CA5"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = "Survey.CA6B7FB1-413D-449B-9038-32AB5B4914B6",
                        IsRequired = true,
                        IsReviewNeeded = true
                    }
                }
            },
        };

        private static TaskVariantDto[] ApacTasks =
        {
            new()
            {
                Type = TaskType.Address,
                VariantId = Guid.Parse("3B349C17-154A-4DC9-A683-045AA985836D"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.ProofOfAddressDocument,
                        IsRequired = false,
                        IsReviewNeeded = true
                    }
                }
            },
            new()
            {
                Type = TaskType.TaxResidence,
                VariantId = Guid.Parse("805BE539-A68E-4E21-9B9E-B9F16DD91C8B"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.W9Form,
                        IsRequired = true,
                        IsReviewNeeded = true
                    }
                }
            }
        };

        private static TaskVariantDto[] UsaTasks =
        {
            new()
            {
                Type = TaskType.Address,
                VariantId = Guid.Parse("828C3D69-D9B1-46D5-9498-45F3DD74B278"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.ProofOfAddressDocument,
                        IsRequired = true,
                        IsReviewNeeded = true
                    }
                }
            },
            new()
            {
                Type = TaskType.UserRiskScreening,
                VariantId = Guid.Parse("C2D32093-AD0F-45AD-9377-4DD12550A221"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = "Survey.EDDACA4C-C4A6-40C6-8FF3-D63A5D435783",
                        IsRequired = true,
                        IsReviewNeeded = true
                    }
                }
            }
        };
        
        private static TaskVariantDto[] RoWTasks =
        {
            new()
            {
                Type = TaskType.Address,
                VariantId = Guid.Parse("7FC98096-1CB1-47FB-91BD-1F600BEB82A0"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.ProofOfAddressDocument,
                        IsRequired = false,
                        IsReviewNeeded = true
                    }
                }
            },
            new()
            {
                Type = TaskType.ProofOfFunds,
                VariantId = Guid.Parse("1673241E-BC0A-4007-A353-A2C39880BBEF"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.ProofOfFundsDocument,
                        IsRequired = false,
                        IsReviewNeeded = true
                    }
                }
            },
            new()
            {
                Type = TaskType.TaxResidence,
                VariantId = Guid.Parse("782BDF03-5C44-45AD-A267-8A26934066A8"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.W9Form,
                        IsRequired = true,
                        IsReviewNeeded = true
                    }
                }
            }
        };

        private static TaskVariantDto[] GlobalTasks =
        {
            new()
            {
                Type = TaskType.TaxResidence,
                VariantId = Guid.Parse("B5160FF6-C9E6-4492-9422-96EB6B8F42EF"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.W9Form,
                        IsRequired = true,
                        IsReviewNeeded = true
                    }
                }
            }
        };
        
        private static TaskVariantDto[] RuTasks =
        {
            new()
            {
                Type = TaskType.Address,
                VariantId = Guid.Parse("2796B0CB-42CB-49B7-A30E-84824D603799"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.ProofOfAddressDocument,
                        IsRequired = true,
                        IsReviewNeeded = true
                    }
                }
            },
            new()
            {
                Type = TaskType.TaxResidence,
                VariantId = Guid.Parse("02F9E694-49F2-4669-A5C8-BEA234D92E03"),
                CollectionSteps = new []
                {
                    new PolicyCollectionStep
                    {
                        XPath = XPathes.W9Form,
                        IsRequired = true,
                        IsReviewNeeded = true
                    }
                }
            }
        };

        private static Dictionary<Guid, TaskVariantDto[]> PolicyDynamicTasks = new()
        {
            { Guid.Parse("0EAAE368-8ACB-410B-8EC0-3AE404F49D5E"), EeaTasks },
            { Guid.Parse("DC658B4F-A0EB-4C20-B296-E0D57E8DA6DB"), GbTasks },
            { Guid.Parse("37C6AD01-067C-4B80-976D-30A568E7B0CD"), ApacTasks },
            { Guid.Parse("4B6271BD-FDE5-40F7-8701-29AA66865568"), UsaTasks },
            { Guid.Parse("D5B5997E-FFC1-495D-9E98-60CCBDD6F43B"), RoWTasks },
            { Guid.Parse("5DECE2A9-CDD3-4D0D-B1BC-8A164B745051"), GlobalTasks },
            { Guid.Parse("67A2B2C8-BEAB-4C3E-A772-19CE9380CB0E"), RuTasks },
        };

        public static TaskVariantDto[] Get(Guid verificationPolicy) =>
            PolicyDynamicTasks[verificationPolicy];
    }
}