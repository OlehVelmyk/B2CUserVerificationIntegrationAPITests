using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Configurations.Options
{
    public class ActionsOption : Option
    {
        public ActionsOption(IEnumerable<RegionActionsOption> regionActionsOptions)
        {
            if (regionActionsOptions == null)
                throw new ArgumentNullException(nameof(regionActionsOptions));

            RegionActions = regionActionsOptions.ToArray();
        }

        public IReadOnlyCollection<RegionActionsOption> RegionActions { get; }
    }

    public class RegionActionsOption
    {
        public RegionActionsOption(RegionType regionType, string region, IEnumerable<ActionOption> actions)
        {
            if (!System.Enum.IsDefined(typeof(RegionType), regionType))
                throw new InvalidEnumArgumentException(nameof(regionType), (int) regionType, typeof(RegionType));

            RegionType = regionType;
            Region = region ?? throw new ArgumentNullException(nameof(region));
            Actions = actions?.ToArray() ?? throw new ArgumentNullException(nameof(actions));
        }

        public RegionType RegionType { get; }

        public string Region { get; }

        public IReadOnlyCollection<ActionOption> Actions { get; }
    }

    public class ActionOption
    {
        public ActionOption(ActionType actionType, string xPath, int priority,
                            IReadOnlyDictionary<string, string> metadata)
        {
            if (priority <= 0)
                throw new ArgumentOutOfRangeException(nameof(priority));
            if (!System.Enum.IsDefined(typeof(ActionType), actionType))
                throw new InvalidEnumArgumentException(nameof(actionType), (int) actionType, typeof(ActionType));

            ActionType = actionType;
            XPath = xPath ?? throw new ArgumentNullException(nameof(xPath));
            Priority = priority;
            Metadata = metadata;
        }

        public ActionType ActionType { get; }

        public string XPath { get; }

        public int Priority { get; }

        public IReadOnlyDictionary<string, string> Metadata { get; }
    }
}