using System;
using WX.B2C.User.Verification.Events.Dtos;
using WX.B2C.User.Verification.Events.Enums;

namespace WX.B2C.User.Verification.Events.EventArgs
{
    public class VerificationApplicationRegisteredEventArgs : System.EventArgs
    {
        public Guid ApplicationId { get; set; }
        
        public Guid UserId { get; set; }
        
        public Guid PolicyId { get; set; }
        
        public ProductType ProductType { get; set; }

        public InitiationDto Initiation { get; set; }

        public static VerificationApplicationRegisteredEventArgs Create(Guid applicationId,
                                                                        Guid userId,
                                                                        Guid policyId,
                                                                        ProductType productType,
                                                                        InitiationDto initiation) =>
            new VerificationApplicationRegisteredEventArgs
            {
                ApplicationId = applicationId,
                UserId = userId,
                PolicyId = policyId,
                ProductType = productType,
                Initiation = initiation
            };
    }
}