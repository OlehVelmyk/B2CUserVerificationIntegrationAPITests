using System.Linq;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Builders
{
    internal class VerificationTaskBuilder
    {
        private VerificationTask _result;

        public VerificationTaskBuilder From(VerificationTaskSpecimen specimen)
        {
            _result = new VerificationTask(
                specimen.Id, 
                specimen.UserId, 
                specimen.VariantId, 
                specimen.Type, 
                specimen.CreationDate,
                specimen.State, 
                specimen.Result, 
                specimen.CollectionSteps?.ToArray(),
                specimen.PerformedChecks?.ToArray(),
                specimen.ExpirationDetails);
            
            return this;
        }
        
        public VerificationTask Build() => _result;
    }
}
