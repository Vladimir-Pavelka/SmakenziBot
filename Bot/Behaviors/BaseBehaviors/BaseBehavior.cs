namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Result;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public abstract class BaseBehavior : IBehavior
    {
        protected MapRegion BasePosition { get; }

        protected BaseBehavior(MapRegion basePosition)
        {
            BasePosition = basePosition;
        }

        public abstract void Execute();

        protected IEnumerable<Unit> EnemiesNearWorkers => Game.Enemy.Units.Where(IsNearWorkers);
        protected bool IsNearWorkers(Unit u) => OwnWorkers.Where(w => w.IsGatheringMinerals || w.IsGatheringGas).Any(w => w.TilePosition.CalcApproximateDistance(u.TilePosition) < 1);

        public IEnumerable<Unit> OwnBelongings => Game.Self.Units.Where(IsInBase);
        public IEnumerable<Unit> OwnUnits => OwnBelongings.Where(x => !x.UnitType.IsBuilding);
        public IEnumerable<Unit> OwnWorkers => OwnUnits.Where(u => u.UnitType.IsWorker);
        public IEnumerable<Unit> OwnFighters => OwnUnits.Where(x => x.IsFighter());
        public IEnumerable<Unit> OwnBuildings => OwnBelongings.Where(x => x.UnitType.IsBuilding);
        public IEnumerable<Unit> OwnStaticDefense => OwnBuildings.Where(x => x.UnitType.CanAttack);

        public IEnumerable<Unit> MineralWorkers => OwnWorkers.Where(w => w.IsGatheringMinerals);
        public IEnumerable<Unit> GasWorkers => OwnWorkers.Where(w => w.IsGatheringGas);

        public IEnumerable<Unit> BaseMinerals => Game.Minerals.Where(IsInBase);
        public IEnumerable<Unit> BaseGeysers => Game.Geysers.Where(IsInBase);
        public IEnumerable<Unit> OwnRefineries => OwnBuildings.Where(b => b.UnitType.IsRefinery);

        public IEnumerable<Unit> EnemiesInBase => Game.Enemy.Units.Where(IsInBase);

        protected bool IsInBase(Unit u) => BasePosition.ContentTiles.Contains(u.Position.ToWalkTile().AsTuple());

        protected bool HasCompletedBuilding(UnitType buildingType) => OwnBuildings.Any(x => x.Is(buildingType) && x.IsCompleted);


        protected void GatherClosestMineral(Unit worker)
        {
            var distanceOrderedMinerals = BaseMinerals.OrderBy(worker.Distance).ToList();
            var closestMineral = distanceOrderedMinerals.FirstOrDefault(x => !x.IsBeingGathered) ?? distanceOrderedMinerals.FirstOrDefault();
            if (closestMineral == null) closestMineral = Game.Minerals.MinBy(worker.Distance);
            if (closestMineral == null) return;
            worker.Gather(closestMineral, false);
        }

        protected void GatherClosestGas(Unit worker)
        {
            var closestGasExtractor = OwnRefineries.OrderBy(worker.Distance).First();
            worker.Gather(closestGasExtractor, false);
        }

        protected Unit GetFreeWorker()
        {
            var freeWorkers = MineralWorkers.Where(x => !x.IsCarryingMinerals).Where(x => !x.IsGatheringGas);
            return freeWorkers.FirstOrDefault() ?? OwnWorkers.FirstOrDefault(x => !x.IsGatheringGas) ?? OwnWorkers.First();
        }
    }

    public class Cache<T>
    {
        private T _value;
        private readonly Func<T> _calculateValue;
        private int _valueFrame = -1;

        public Cache(Func<T> calculateValue)
        {
            _calculateValue = calculateValue;
        }

        private T Value
        {
            get
            {
                if (Game.FrameCount > _valueFrame) _value = _calculateValue();
                return _value;
            }
        }
    }
}