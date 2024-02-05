using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Component.Tests.Automation
{
    internal class FailResultTestCases
    {
        /// <summary>
        /// Scenario: Request PoA step
        /// Given user with ip 
        /// And ip location differs from residence country
        /// When IpMatch check is failed
        /// Then PoA collection step is requested
        /// And event is raised
        /// </summary>
        public async Task ShouldRequestPoA_WhenIpMatchCheckFailed()
        {
        }

        /// <summary>
        /// Scenario: Request W9Form step
        /// Given user with US tax residence  
        /// When TaxResidence check is failed
        /// Then W9Form collection step is requested
        /// And event is raised
        /// </summary>
        public async Task ShouldRequestW9Form_WhenTaxResidenceCheckFailed()
        {
        }

        /// <summary>
        /// Scenario: Request PoA step
        /// Given user 
        /// When IdentityEnhanced check is failed
        /// Then PoA collection step is requested
        /// And event is raised
        /// </summary>
        public async Task ShouldRequestPoA_WhenIdentityEnhancedCheckFailed()
        {
        }

        /// <summary>
        /// Scenario: Request PEP survey step
        /// Given PEP user 
        /// When RiskListsScreening check is failed
        /// Then PEP survey collection step is requested
        /// And event is raised
        /// </summary>
        public async Task ShouldRequestPepSurvey_WhenRiskListsScreeningCheckFailed()
        {
        }
    }
}
