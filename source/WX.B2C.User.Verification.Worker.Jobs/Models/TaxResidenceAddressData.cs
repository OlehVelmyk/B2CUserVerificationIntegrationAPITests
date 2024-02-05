using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    public class TaxResidenceAddressData 
    {
        public Guid UserId { get; set; }

        public string Country { get; set; }
    }
}