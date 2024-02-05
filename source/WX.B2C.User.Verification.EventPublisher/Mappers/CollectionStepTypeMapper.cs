using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Events.Constants;
using WX.B2C.User.Verification.Events.EventArgs;

namespace WX.B2C.User.Verification.EventPublisher.Mappers
{
    internal interface ICollectionStepEventMapper
    {
        CollectionStepReadyForReviewEventArgs Map(CollectionStepReadyForReview @event);

        CollectionStepCompletedEventArgs Map(CollectionStepCompleted @event);

        CollectionStepRequestedEventArgs Map(CollectionStepRequested @event);
    }

    internal class CollectionStepEventMapper : ICollectionStepEventMapper
    {
        private readonly IXPathParser _xPathParser;

        public CollectionStepEventMapper(IXPathParser xPathParser)
        {
            _xPathParser = xPathParser ?? throw new ArgumentNullException(nameof(xPathParser));
        }

        public CollectionStepReadyForReviewEventArgs Map(CollectionStepReadyForReview @event)
        {
            var xPathDetails = _xPathParser.Parse(@event.XPath);

            return xPathDetails switch
            {
                DocumentsXPathDetails { Category: DocumentCategory.ProofOfAddress } =>
                    new CollectionStepReadyForReviewEventArgs
                    {
                        StepType = CollectionStepTypes.ProofOfAddress,
                        UserId = @event.UserId
                    },
                SurveyXPathDetails =>
                    new CollectionStepReadyForReviewEventArgs
                    {
                        StepType = CollectionStepTypes.Survey,
                        UserId = @event.UserId
                    },
                _ => null
            };
        }

        public CollectionStepCompletedEventArgs Map(CollectionStepCompleted @event)
        {
            var xPathDetails = _xPathParser.Parse(@event.XPath);

            return xPathDetails switch
            {
                DocumentsXPathDetails { Category: DocumentCategory.ProofOfAddress } =>
                    new CollectionStepCompletedEventArgs
                    {
                        StepType = CollectionStepTypes.ProofOfAddress,
                        UserId = @event.UserId
                    },
                SurveyXPathDetails =>
                    new CollectionStepCompletedEventArgs
                    {
                        StepType = CollectionStepTypes.Survey,
                        UserId = @event.UserId
                    },
                _ => null
            };
        }

        public CollectionStepRequestedEventArgs Map(CollectionStepRequested @event)
        {
            var xPathDetails = _xPathParser.Parse(@event.XPath);

            return xPathDetails switch
            {
                DocumentsXPathDetails { Category: DocumentCategory.ProofOfAddress } =>
                    new CollectionStepRequestedEventArgs
                    {
                        StepType = CollectionStepTypes.ProofOfAddress,
                        UserId = @event.UserId
                    },
                SurveyXPathDetails surveyXPathDetails =>
                    new SurveyRequestedEventArgs
                    {
                        TemplateId = surveyXPathDetails.SurveyId,
                        StepType = CollectionStepTypes.ProofOfAddress,
                        UserId = @event.UserId
                    },
                _ => null
            };
        }
    }
}