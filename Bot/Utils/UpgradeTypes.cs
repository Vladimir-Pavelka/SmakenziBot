namespace SmakenziBot.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api.Enum;

    public static class UpgradeTypes
    {
        public static IReadOnlyDictionary<UpgradeType, BroodWar.Api.Upgrade> All { get; } =
            BroodWar.Api.Upgrade.AllUpgrades.ToDictionary(x => x.Type, x => x);
    }
}