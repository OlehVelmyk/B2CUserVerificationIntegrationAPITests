using System;
using System.Linq;
using WX.B2C.Survey.Api.Internal.Client.Models;
using WX.B2C.User.Verification.Core.Contracts;

namespace WX.B2C.User.Verification.B2C.Survey.Mappers
{
    public interface IUserSurveyMapper
    {
        TaggedAnswer Map(TaggedAnswerDto dto);
    }

    internal class UserSurveyMapper : IUserSurveyMapper
    {
        public TaggedAnswer Map(TaggedAnswerDto taggedAnswer)
        {
            if (taggedAnswer == null)
                throw new ArgumentNullException(nameof(taggedAnswer));

            var tags = taggedAnswer.Tags ?? throw new ArgumentNullException(nameof(taggedAnswer.Tags));
            var answers = taggedAnswer.Values ?? throw new ArgumentNullException(nameof(taggedAnswer.Values));

            return new TaggedAnswer
            {
                Tags = tags.ToArray(),
                Values = answers.Select(answer => answer.Value).ToArray()
            };
        }
    }
}