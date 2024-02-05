using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Survey.Mappers;
using WX.Survey.Api.Client;

namespace WX.B2C.User.Verification.Survey
{
    internal class SurveyTemplatesProvider : ISurveyTemplatesProvider
    {
        private readonly ISurveyApiClientFactory _clientFactory;
        private readonly IUserSurveyTemplateMapper _userSurveyTemplateMapper;
        
        public SurveyTemplatesProvider(ISurveyApiClientFactory clientFactory, IUserSurveyTemplateMapper userSurveyTemplateMapper)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _userSurveyTemplateMapper = userSurveyTemplateMapper ?? throw new ArgumentNullException(nameof(userSurveyTemplateMapper));
        }
        
        public async Task<SurveyTemplateDto[]> GetAsync()
        {
            using var apiClient = _clientFactory.Create();
            
            var surveys = await apiClient.Surveys.GetSurveysAsync();
            
            return surveys.Select(_userSurveyTemplateMapper.Map).ToArray();
        }

        public async Task<SurveyTemplateDto[]> GetAsync(Guid[] templateIds)
        {
            if (templateIds.IsNullOrEmpty())
                return Array.Empty<SurveyTemplateDto>();

            using var apiClient = _clientFactory.Create();

            var surveys = await apiClient.Surveys.GetSurveysAsync();

            return templateIds
                   .Join(
                       surveys, templateId => templateId,
                       template => template.Id,
                       (_, template) => _userSurveyTemplateMapper.Map(template))
                   .ToArray();
        }
    }
}
