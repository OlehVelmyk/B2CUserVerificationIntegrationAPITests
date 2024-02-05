using System.Threading.Tasks;
using MediatR;
using Serilog;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Requests;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Handlers
{
    public class ManualActionRequired : IRequest
    {
        public ActionDto[] Actions { get; set; }
    }

    internal class ManualActionRequiredHandler : ForgettableHandler<ManualActionRequired>
    {
        public ManualActionRequiredHandler(ILogger logger)
            : base(logger)
        {
        }

        protected override Task Handle(ManualActionRequired request)
        {
            // TODO: Should be implemented
            return Task.FromResult(Unit.Value);
        }
    }
}
