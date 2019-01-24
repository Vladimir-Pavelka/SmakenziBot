namespace SmakenziBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Result;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class MyBase
    {
        public MapRegion BaseRegion { get; }
        private (int, int) ResourceDepotSite => BaseRegion.ResourceSites.First().OptimalResourceDepotBuildTile;

        public MyBase(MapRegion baseRegion, BaseType baseType)
        {
            BaseRegion = baseRegion;
            BaseType = baseType;
        }

        public BaseType BaseType { get; }
        public bool IsMain => BaseType == BaseType.Main;
        public bool IsNatural => BaseType == BaseType.Natural;
        public bool IsThird => BaseType == BaseType.Third;
        public bool IsNthExpansion => BaseType == BaseType.Nth;

        public IEnumerable<Unit> OwnBelongings => Game.Self.Units.Where(IsInBase);
        public IEnumerable<Unit> OwnUnits => OwnBelongings.Where(x => !x.UnitType.IsBuilding);
        public IEnumerable<Unit> OwnWorkers => OwnUnits.Where(u => u.UnitType.IsWorker);
        public IEnumerable<Unit> OwnFighters => OwnUnits.Where(x => x.IsFighter());
        public IEnumerable<Unit> OwnBuildings => OwnBelongings.Where(x => x.UnitType.IsBuilding);
        public IEnumerable<Unit> OwnStaticDefense => OwnBuildings.Where(x => x.UnitType.CanAttack);
        public IEnumerable<Unit> OwnHatcheries => OwnBuildings.Where(x => x.UnitType.ProducesLarva);

        public IEnumerable<Unit> MineralWorkers => OwnWorkers.Where(w => w.IsGatheringMinerals);
        public IEnumerable<Unit> GasWorkers => OwnWorkers.Where(w => w.IsGatheringGas);

        public IEnumerable<Unit> BaseMinerals => Game.Minerals.Where(IsInBase);
        public IEnumerable<Unit> BaseGeysers => Game.Geysers.Where(IsInBase);
        public IEnumerable<Unit> OwnRefineries => OwnBuildings.Where(b => b.UnitType.IsRefinery);

        public IEnumerable<Unit> EnemiesInBase => Game.Enemy.Units.Where(IsInBase);

        public bool IsInBase(Unit u) => BaseRegion.ContentTiles.Contains(u.Position.ToWalkTile().AsTuple());
        public bool HasCompletedBuilding(UnitType buildingType) => OwnBuildings.Any(x => x.Is(buildingType) && x.IsCompleted);

        public override int GetHashCode() => ResourceDepotSite.GetHashCode();
        public override bool Equals(object other)
        {
            if (!(other is MyBase otherMyBase)) return false;
            return ReferenceEquals(this, otherMyBase) || Equals(otherMyBase);
        }
        private bool Equals(MyBase other) => ResourceDepotSite.Equals(other.ResourceDepotSite);
    }

    public enum BaseType
    {
        Main,
        Natural,
        Third,
        Nth
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