using System.Collections.Generic;
using System.Net;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class IpAddressCheckDataValidator : BaseCheckInputValidator<IpAddressCheckData>
    {
        public IpAddressCheckDataValidator(IpAddressCheckConfiguration configuration)
            : base(configuration)
        {
        }

        protected override IEnumerable<string> Validate(CheckInputData inputData, out IpAddressCheckData validatedData)
        {
            var missingData = new List<string>();

            var address = inputData.TryGetValue<string>(XPathes.IpAddress, missingData);
            IPAddress ipAddress = null;
            if (address != null && !IPAddress.TryParse(address, out ipAddress))
            {
                missingData.Add($"Cannot parse ip address {address}");
            }

            validatedData = new IpAddressCheckData
            {
                IpAddress = ipAddress,
                ResidenceAddress = inputData.TryGetValue<AddressDto>(XPathes.ResidenceAddress, missingData)
            };

            return missingData;
        }
    }
}
