using WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    internal class InitiateWorkflowContainer
    {
        public InitiateWorkflowRequest Request { get; set; }

        public int? Cvi { get; set; }

        public string Hri { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}