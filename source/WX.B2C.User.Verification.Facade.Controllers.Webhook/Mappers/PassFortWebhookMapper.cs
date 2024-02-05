using System;
using MediatR;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Handlers;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Requests;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Mappers
{
    public interface IPassFortWebhookMapper
    {
        IRequest Map(PassFortEventRequest eventRequest);
    }

    internal class PassFortWebhookMapper : IPassFortWebhookMapper
    {
        public IRequest Map(PassFortEventRequest eventRequest)
        {
            if (eventRequest == null)
                throw new ArgumentNullException(nameof(eventRequest));

            return eventRequest switch
            {
                PassFortCheckCompletedEventRequest request => Map(request.Data),
                PassFortProductStatusChangedEventRequest request => Map(request.Data),
                PassFortManualActionRequiredEventRequest request => Map(request.Data),
                _ => throw new ArgumentOutOfRangeException(nameof(eventRequest), eventRequest, "Unsupported event data type.")
            };
        }

        private static ManualActionRequired Map(ManualActionRequiredData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return new ManualActionRequired
            {
                Actions = data.Actions
            };
        }

        private static ChangeProductStatus Map(ProductStatusChangedData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return new ChangeProductStatus
            {
                ApplicationId = data.ApplicationId,
                ProductId = data.Product.Id,
                ProductName = data.Product.Name,
                NewStatus = data.NewStatus
            };
        }

        private static CompletePassFortCheck Map(CheckCompletedData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return new CompletePassFortCheck
            {
                CheckType = data.Check.CheckType,
                CheckId = data.Check.Id,
                CheckResult = data.Check.Result
            };
        }
    }
}