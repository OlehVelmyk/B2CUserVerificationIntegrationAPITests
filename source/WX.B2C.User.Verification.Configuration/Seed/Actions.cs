using System;
using WX.B2C.User.Verification.Configuration.Models;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.XPath;
using static WX.B2C.User.Verification.Core.Contracts.Enum.ActionType;
using Action = WX.B2C.User.Verification.Configuration.Models.Action;

namespace WX.B2C.User.Verification.Configuration.Seed
{
    internal static class Actions
    {
        public static RegionActions[] Seed =
        {
            new()
            {
                RegionType = RegionType.Global,
                Region = "Global",
                Actions = new Action[]
                {
                    new()
                    {
                        ActionType = ProofOfIdentity,
                        XPath = XPathes.ProofOfIdentityDocument,
                        Priority = 1
                    },
                    new()
                    {
                        ActionType = Selfie,
                        XPath = XPathes.SelfiePhoto,
                        Metadata = new()
                        {
                            { "SelfieType", "Photo" }
                        },
                        Priority = 2
                    },
                    new()
                    {
                        ActionType = Selfie,
                        XPath = XPathes.SelfieVideo,
                        Metadata = new()
                        {
                            { "SelfieType", "Video" }
                        },
                        Priority = 3
                    },
                    new()
                    {
                        ActionType = Tin,
                        XPath = XPathes.Tin,
                        Priority = 4
                    },
                    new()
                    {
                        ActionType = TaxResidence,
                        XPath = XPathes.TaxResidence,
                        Priority = 5
                    },
                    new()
                    {
                        ActionType = W9Form,
                        XPath = XPathes.W9Form,
                        Priority = 6
                    },
                    new()
                    {
                        //UK Onboarding survey
                        ActionType = Survey,
                        XPath = new SurveyXPath(Guid.Parse("C5E7A138-2E36-43D0-BD76-43A606068F49")),
                        Metadata = new()
                        {
                            { "SurveyId", "C5E7A138-2E36-43D0-BD76-43A606068F49" },
                            { "SurveyTag", "clientUkOnboarding" }
                        },
                        Priority = 7
                    },
                    new()
                    {
                        ActionType = ProofOfAddress,
                        XPath = XPathes.ProofOfAddressDocument,
                        Priority = 8
                    },
                    new()
                    {
                        //UK PEP survey
                        ActionType = Survey,
                        XPath = new SurveyXPath(Guid.Parse("CA6B7FB1-413D-449B-9038-32AB5B4914B6")),
                        Metadata = new()
                        {
                            { "SurveyId", "CA6B7FB1-413D-449B-9038-32AB5B4914B6" },
                            { "SurveyTag", "clientUkPep" }
                        },
                        Priority = 9
                    },
                    new()
                    {
                        //UK Income-Occupation survey
                        ActionType = Survey,
                        XPath = new SurveyXPath(Guid.Parse("F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686")),
                        Metadata = new()
                        {
                            { "SurveyId", "F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686" },
                            { "SurveyTag", "clientUkOccupation" }
                        },
                        Priority = 10
                    },
                    new()
                    {
                        //US CDD survey
                        ActionType = Survey,
                        XPath = new SurveyXPath(Guid.Parse("DE532CA0-C21E-4F7B-AD09-647EAA0C4E00")),
                        Metadata = new()
                        {
                            { "SurveyId", "DE532CA0-C21E-4F7B-AD09-647EAA0C4E00" },
                            { "SurveyTag", "clientUsCdd" }
                        },
                        Priority = 11
                    },
                    new()
                    {
                        //US EDD survey
                        ActionType = Survey,
                        XPath = new SurveyXPath(Guid.Parse("EDDACA4C-C4A6-40C6-8FF3-D63A5D435783")),
                        Metadata = new()
                        {
                            { "SurveyId", "EDDACA4C-C4A6-40C6-8FF3-D63A5D435783" },
                            { "SurveyTag", "clientUsEdd" }
                        },
                        Priority = 12
                    },
                    new()
                    {
                        //UK SoF survey
                        ActionType = Survey,
                        XPath = new SurveyXPath(Guid.Parse("0FB7492B-7DC5-4277-A7FF-F3D07376FF66")),
                        Metadata = new()
                        {
                            { "SurveyId", "0FB7492B-7DC5-4277-A7FF-F3D07376FF66" },
                            { "SurveyTag", "clientUkSof" }
                        },
                        Priority = 13
                    },
                    new()
                    {
                        ActionType = ProofOfFunds,
                        XPath = XPathes.ProofOfFundsDocument,
                        Priority = 14
                    }
                }
            }
        };
    }
}