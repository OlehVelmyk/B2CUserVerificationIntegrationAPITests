using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public class TaggedAnswer
    {
        public string[] Tags { get; set; }

        public string[] Values { get; set; }
    }

    public interface IUserSurveyProvider
    {
        Task<IList<TaggedAnswer>> GetAnswersAsync(Guid userId, Guid templateId, IList<string> tags);
    }
}