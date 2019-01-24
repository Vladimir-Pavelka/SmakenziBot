namespace SmakenziBot
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Result;
    using Utils;

    public class GameInfo
    {
        private readonly AnalyzedMap _analyzedMap;
        private readonly List<MyBase> _registeredOwnBases = new List<MyBase>();

        public GameInfo(AnalyzedMap analyzedMap)
        {
            _analyzedMap = analyzedMap;
        }

        public IReadOnlyCollection<MyBase> MyBases => _registeredOwnBases;
        public void RegisterMyBase(MyBase b) => _registeredOwnBases.Add(b);

        private static bool ContainsOwnResourceDepot(ResourceSite rs)
        {
            var resourceDepotBuildTile = rs.OptimalResourceDepotBuildTile.AsBuildTile();
            return Game.GetUnitsOnTile(resourceDepotBuildTile).Where(Game.Self.Units.Contains)
                .Any(u => u.UnitType.IsResourceDepot);
        }
    }
}