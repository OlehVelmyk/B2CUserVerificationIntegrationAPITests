using System;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Fixtures
{
    internal class OnfidoTokenFixture
    {
        public SdkTokenRequest Create(string applicantId) =>
            new SdkTokenRequest
            {
                ApplicantId = applicantId,
                ApplicationId = Guid.NewGuid().ToString()

            };        

        public SdkTokenRequest Create(string applicantId, string referrer) =>
            new SdkTokenRequest
            {
                ApplicantId = applicantId,
                Referrer = referrer
            };        
    }
}
