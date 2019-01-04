namespace SmakenziBot
{
    using System.Collections.Generic;
    using BroodWar.Api;

    public static class GameMemory
    {
        public static HashSet<Position> EnemyBuildings { get; } = new HashSet<Position>();
    }
}