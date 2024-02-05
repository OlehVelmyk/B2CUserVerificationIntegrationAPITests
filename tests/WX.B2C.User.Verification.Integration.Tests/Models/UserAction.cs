using System.ComponentModel;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;

namespace WX.B2C.User.Verification.Integration.Tests.Models;

internal class UserAction
{
    private readonly ActionType _actionType;
    private readonly SurveyType? _surveyType;
    private readonly bool _isOptional;
    
    public UserAction(ActionType actionType, bool isOptional = false, SurveyType? surveyType = null)
    {
        if (surveyType is not null && actionType is not ActionType.Survey)
            throw new InvalidEnumArgumentException($"{nameof(surveyType)} is only applicable for {ActionType.Survey}");
        
        _actionType = actionType;
        _isOptional = isOptional;
        _surveyType = surveyType;
    }

    public bool Equals(UserActionDto actionDto) =>
        actionDto.ActionType == _actionType &&
        actionDto.IsOptional == _isOptional &&
        (_surveyType is null || new Guid(actionDto.ActionData["survey_id"]) == SurveyHelper.GetSurveyId(_surveyType.Value));

    public override string ToString() =>
        $"{nameof(_actionType)}: {_actionType}, " +
        $"{nameof(_surveyType)}: {_surveyType}, " +
        $"{nameof(_isOptional)}: {_isOptional}";
}
