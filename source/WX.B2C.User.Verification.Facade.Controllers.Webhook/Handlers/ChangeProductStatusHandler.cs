using System.Threading.Tasks;
using MediatR;
using Serilog;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Handlers
{
    public class ChangeProductStatus : IRequest
    {
        public string ApplicationId { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public string NewStatus { get; set; }
    }

    internal class ChangeProductStatusHandler : ForgettableHandler<ChangeProductStatus>
    {
        public ChangeProductStatusHandler(ILogger logger)
            : base(logger)
        {
        }

        protected override Task Handle(ChangeProductStatus request)
        {
            // TODO: Should be implemented
            return Task.FromResult(Unit.Value);
        }
    }
}
