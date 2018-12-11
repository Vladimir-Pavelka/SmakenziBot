namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public abstract class BaseBehavior : IBehavior
    {
        protected TilePosition BasePosition { get; }

        protected BaseBehavior(TilePosition basePosition)
        {
            BasePosition = basePosition;
        }

        public abstract void Execute();

        protected IReadOnlyCollection<Unit> EnemiesNearMineralLine => Game.Enemy.Units.Where(IsNearMineralLine).ToList();

        protected IEnumerable<Unit> EnemiesInBase => Game.Enemy.Units.Where(IsInBase);
        protected IEnumerable<Unit> WorkersNearMineralLine => BaseWorkers.Where(IsNearMineralLine);
        protected bool IsInBase(Unit u) => u.TilePosition.CalcApproximateDistance(BasePosition) < 15;
        protected bool IsNearMineralLine(Unit u) => BaseWorkers.Where(w => w.IsGatheringMinerals || w.IsGatheringGas).Any(w => w.TilePosition.CalcApproximateDistance(u.TilePosition) < 1);

        protected IEnumerable<Unit> BaseWorkers => Game.Self.Units.Where(u => u.UnitType.IsWorker).Where(IsInBase);
        protected IEnumerable<Unit> BaseMinerals => Game.Minerals.Where(IsInBase);

        protected bool HasBuilding(UnitType buildingType) => BaseBuildings.Any(x => x.UnitType.Type == buildingType);
        protected IEnumerable<Unit> BaseBuildings => Game.Self.Units.Where(x => x.UnitType.IsBuilding).Where(x => x.IsCompleted).Where(IsInBase);

        protected IEnumerable<Unit> BaseCombatUnits => Game.Self.Units.Where(x => x.IsFighter()).Where(IsInBase);

        protected void GatherClosestMineral(Unit worker)
        {
            var distanceOrderedMinerals = BaseMinerals.OrderBy(worker.Distance).ToList();
            var closestMineral = distanceOrderedMinerals.FirstOrDefault(x => !x.IsBeingGathered) ?? distanceOrderedMinerals.First();
            worker.Gather(closestMineral, false);
        }

        protected void GatherClosestGas(Unit worker)
        {
            var closestGasExtractor = BaseBuildings.Where(b => b.UnitType.Type == UnitType.Zerg_Extractor).OrderBy(worker.Distance).First();
            worker.Gather(closestGasExtractor, false);
        }

        protected Unit GetWorker()
        {
            var freeWorkers = BaseWorkers.Where(x => !x.IsCarryingMinerals).Where(x => !x.IsGatheringGas);
            return freeWorkers.FirstOrDefault() ?? BaseWorkers.FirstOrDefault(x => !x.IsGatheringGas) ?? BaseWorkers.First();
        }
    }
}