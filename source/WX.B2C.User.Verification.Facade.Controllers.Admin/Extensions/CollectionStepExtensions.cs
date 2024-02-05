using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Extensions
{
    public static class CollectionStepExtensions
    {
        public static Dictionary<string, CollectionStepDto> ToDictionary(this IEnumerable<CollectionStepDto> allSteps)
        {
            if (allSteps == null)
                throw new ArgumentNullException(nameof(allSteps));

            return allSteps
                   .GroupBy(x => x.XPath)
                   .ToDictionary(x => x.Key, FirstWithHighestPriority);
        }

        private static CollectionStepDto FirstWithHighestPriority(IGrouping<string, CollectionStepDto> steps)
        {
            return steps.OrderBy(dto => dto.XPath)
                        .ThenBy(dto => dto.State)
                        .ThenByDescending(dto => dto.IsRequired)
                        .First();
        }
    }
}