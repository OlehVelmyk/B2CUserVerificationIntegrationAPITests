using System.Collections.Generic;
using System.Net;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using WX.B2C.User.Verification.Provider.Contracts.Stubs;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal class IpAddressCheckProcessingResultFactory : ICheckProcessingResultFactory
    {
        public CheckProcessingResult Create(CheckInputData inputData)
        {
            var missingData = new List<string>();

            var ipAddress = inputData.TryGetValue<string>(XPathes.IpAddress, missingData); ;
            var address = inputData.TryGetValue<AddressDto>(XPathes.ResidenceAddress, missingData);

            if (missingData.Count > 0)
                throw new CheckInputValidationException(missingData.ToArray());

            var isIpMatched = ipAddress != IPAddress.None.ToString();
            var checkOutput = new IpAddressCheckOutputData
            {
                IsIpMatched = isIpMatched,
                ResolvedLocation = isIpMatched
                    ? new IpAddressLocation { City = address.City }
                    : null
            };

            return isIpMatched ? CheckProcessingResult.Passed(checkOutput) : CheckProcessingResult.Failed(checkOutput);
        }
    }
}
