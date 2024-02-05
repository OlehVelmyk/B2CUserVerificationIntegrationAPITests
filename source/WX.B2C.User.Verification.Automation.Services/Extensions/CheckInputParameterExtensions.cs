using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Automation.Services.Extensions
{
    internal static class CheckInputParameterExtensions
    {
        public static bool MatchesXPath(this IEnumerable<CheckInputParameterDto> checkParameters, params string[] xPathes)
        {
            if (checkParameters == null)
                throw new ArgumentNullException(nameof(checkParameters));
            if (xPathes == null)
                throw new ArgumentNullException(nameof(xPathes));

            return checkParameters.Any(parameter => xPathes.Contains(parameter.XPath));
        }
    }
}