using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface ISurveyTemplatesProvider
    {
        Task<SurveyTemplateDto[]> GetAsync();

        Task<SurveyTemplateDto[]> GetAsync(Guid[] templateIds);
    }
}
