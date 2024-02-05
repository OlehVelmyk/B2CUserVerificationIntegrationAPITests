using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Survey
{
    public class SurveyTemplatesProviderStub : ISurveyTemplatesProvider
    {
        public Task<SurveyTemplateDto[]> GetAsync()
        {
            return Task.FromResult(_surveyTemplates);
        }

        public Task<SurveyTemplateDto[]> GetAsync(Guid[] templateIds)
        {
            if (templateIds.IsNullOrEmpty())
                return Task.FromResult(Array.Empty<SurveyTemplateDto>());

            return Task.FromResult(_surveyTemplates.Where(surveyTemplate => templateIds.Any(id => id == surveyTemplate.TemplateId)).ToArray());
        }

        private readonly SurveyTemplateDto[] _surveyTemplates =
        {
            new()
            {
                TemplateId = new Guid("DE532CA0-C21E-4F7B-AD09-647EAA0C4E00"),
                Name = "US CDD Survey"
            },
            new()
            {
                TemplateId = new Guid("EDDACA4C-C4A6-40C6-8FF3-D63A5D435783"),
                Name = "US EDD Survey"
            },
            new()
            {
                TemplateId = new Guid("DD438D61-34DB-4300-9778-1A5BB4B17F36"),
                Name = "UK CDD Survey"
            },
            new()
            {
                TemplateId = new Guid("C5E7A138-2E36-43D0-BD76-43A606068F49"),
                Name = "UK Onboarding Survey"
            },
            new()
            {
                TemplateId = new Guid("F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686"),
                Name = "UK Salary and Occupation Survey"
            },
            new()
            {
                TemplateId = new Guid("0FB7492B-7DC5-4277-A7FF-F3D07376FF66"),
                Name = "UK SoF Survey"
            },
            new()
            {
                TemplateId = new Guid("CA6B7FB1-413D-449B-9038-32AB5B4914B6"),
                Name = "UK PEP Survey"
            },
            new()
            {
                TemplateId = new Guid("CCE54FC6-076C-4B02-BEA2-C52278794EE1"),
                Name = "User Feedback Survey"
            }
        };
    }
}
