using System.Linq;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Builders
{
    internal class ApplicationBuilder
    {
        private Application _result;

        public ApplicationBuilder From(ApplicationSpecimen specimen)
        {
            _result = new Application(specimen.Id, specimen.UserId, specimen.PolicyId, specimen.ProductType,
                                      specimen.State, specimen.PreviousState, specimen.RequiredTasks?.Select(MapTask).ToArray(),
                                      specimen.DecisionReasons.ToArray(), true);
            return this;
        }

        public Application Build() =>
            _result;

        private ApplicationTask MapTask(VerificationTaskSpecimen task) =>
            new ApplicationTask(task.Id, task.State, task.Type);
    }
}
