namespace SmakenziBot.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api.Enum;

    public static class UnitTypes
    {
        public static IReadOnlyDictionary<UnitType, BroodWar.Api.UnitType> All { get; } =
            BroodWar.Api.UnitType.AllUnitTypes.ToDictionary(x => x.Type, x => x);
    }
}