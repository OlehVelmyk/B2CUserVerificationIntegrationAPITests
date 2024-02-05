using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WX.B2C.Survey.Api.Internal.Client;
using WX.B2C.Survey.Api.Internal.Client.Models;
using WX.B2C.User.Verification.B2C.Survey.Mappers;
using WX.B2C.User.Verification.Core.Contracts;

namespace WX.B2C.User.Verification.B2C.Survey
{
    internal class UserSurveyProvider : IUserSurveyProvider
    {
        private readonly IB2CSurveyApiClientFactory _clientFactory;
        private readonly IUserSurveyMapper _userSurveyMapper;
        
        public UserSurveyProvider(IB2CSurveyApiClientFactory clientFactory, IUserSurveyMapper userSurveyMapper)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _userSurveyMapper = userSurveyMapper ?? throw new ArgumentNullException(nameof(userSurveyMapper));
        }

        public async Task<IList<TaggedAnswer>> GetAnswersAsync(Guid userId, Guid templateId, IList<string> tags)
        {
            using var apiClient = _clientFactory.Create();

            try
            {
                var answers = await apiClient.UserSurveys.GetAnswersAsync(userId, templateId, tags);
                return answers.Select(_userSurveyMapper.Map).ToArray();
            }
            //TODO: https://wirexapp.atlassian.net/browse/WRXB-10707
            catch (ErrorResponseException e) when(e.Response.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<TaggedAnswer>(0);
            }
        }
    }
}