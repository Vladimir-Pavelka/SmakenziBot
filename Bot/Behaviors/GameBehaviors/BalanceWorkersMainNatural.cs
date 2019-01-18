namespace SmakenziBot.Behaviors.GameBehaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Result;
    using Utils;

    public class BalanceWorkersMainNatural : GameBehavior
    {
        private readonly MapRegion _main;
        private readonly MapRegion _natural;
        private readonly int _mainIdealMineralWorkerCount;
        private readonly int _naturalIdealMineralWorkerCount;
        private const double WorkerToMineralCountRatio = 1.5;

        public BalanceWorkersMainNatural(MapRegion main, MapRegion natural)
        {
            _main = main;
            _mainIdealMineralWorkerCount = Round(
                _main.ResourceSites.First().MineralsBuildTiles.Count * WorkerToMineralCountRatio);
            _natural = natural;
            _naturalIdealMineralWorkerCount =
                Round(_natural.ResourceSites.First().MineralsBuildTiles.Count * WorkerToMineralCountRatio);
        }

        private static int Round(double x) => (int)Math.Round(x);

        public override void Execute()
        {
            if (Game.FrameCount % 150 != 0) return;
            if (!HasResourceDepot(_natural)) return;
            if (!BaseMinerals(_main).Any() || !BaseMinerals(_natural).Any()) return;

            var mainMineralWorkers = BaseWorkers(_main).Where(w => w.IsGatheringMinerals).ToList();
            if (!mainMineralWorkers.Any()) return;
            var naturalMineralWorkers = BaseWorkers(_natural).Where(w => w.IsGatheringMinerals).ToList();

            var mainWorkersRatio = (double) mainMineralWorkers.Count / _mainIdealMineralWorkerCount;
            var naturalWorkersRatio = (double)naturalMineralWorkers.Count / _naturalIdealMineralWorkerCount;
            if (mainWorkersRatio > naturalWorkersRatio + 0.1) mainMineralWorkers.First().Gather(BaseMinerals(_natural).First(), false);
            if (naturalWorkersRatio > mainWorkersRatio + 0.1) naturalMineralWorkers.First().Gather(BaseMinerals(_main).First(), false);
        }

        private IEnumerable<Unit> BaseWorkers(MapRegion r) =>
            Game.Self.Units.Where(u => u.UnitType.IsWorker).Where(w => IsInBase(w, r)).ToList();

        protected bool IsInBase(Unit u, MapRegion r) => r.ContentTiles.Contains(u.Position.ToWalkTile().AsTuple());

        private static bool HasResourceDepot(MapRegion r) =>
            Game.GetUnitsOnTile(r.ResourceSites.First().OptimalResourceDepotBuildTile.AsBuildTile())
            .Any(u => u.UnitType.IsResourceDepot && !u.IsBeingConstructed);

        private static IEnumerable<Unit> BaseMinerals(MapRegion r) =>
            Game.Minerals.Where(m => r.ContentTiles.Contains(m.TilePosition.ToWalkTile().AsTuple()));
    }
}