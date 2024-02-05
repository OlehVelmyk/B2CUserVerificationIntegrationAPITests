using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface ISurveyMapper
    {
        Dtos.SurveyTemplateDto Map(SurveyTemplateDto templateDto);
    }

    internal class SurveyMapper : ISurveyMapper
    {
        public Dtos.SurveyTemplateDto Map(SurveyTemplateDto templateDto)
        {
            if (templateDto == null)
                throw new ArgumentNullException(nameof(templateDto));

            return new Dtos.SurveyTemplateDto
            {
                Name = templateDto.Name,
                TemplateId = templateDto.TemplateId
            };
        }
    }
}