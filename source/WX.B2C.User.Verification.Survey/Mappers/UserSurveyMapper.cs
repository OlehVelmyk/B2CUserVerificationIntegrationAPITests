using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.Survey.Api.Client.Models;

namespace WX.B2C.User.Verification.Survey.Mappers
{ 
    public interface IUserSurveyTemplateMapper
    {
        SurveyTemplateDto Map(SurveyBriefDto dto);
    }
    
    internal class UserSurveyTemplateMapper : IUserSurveyTemplateMapper
    {
        public SurveyTemplateDto Map(SurveyBriefDto dto)
        {
            return new SurveyTemplateDto
            {
                Name = dto.Title,
                TemplateId = dto.Id
            };
        }
    }
}