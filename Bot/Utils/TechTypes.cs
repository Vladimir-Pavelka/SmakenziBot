namespace SmakenziBot.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api.Enum;

    public static class TechTypes
    {
        public static IReadOnlyDictionary<TechType, BroodWar.Api.Tech> All { get; } =
            BroodWar.Api.Tech.AllTechs.ToDictionary(x => x.Type, x => x);
    }
}