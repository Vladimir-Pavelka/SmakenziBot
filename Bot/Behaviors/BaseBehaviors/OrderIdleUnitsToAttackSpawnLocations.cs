namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class OrderIdleUnitsToAttackSpawnLocations : BaseBehavior
    {
        private readonly UnitType _unitType;
        private readonly int _minCount;

        public OrderIdleUnitsToAttackSpawnLocations(UnitType unitType, int minCount, TilePosition basePosition) : base(basePosition)
        {
            _unitType = unitType;
            _minCount = minCount;
        }

        public override void Execute()
        {
            if (Game.FrameCount % 20 != 0) return;
            var idleUnitsOfWantedTypeInMyBase =
                BaseCombatUnits.Where(x => x.UnitType.Type == _unitType).Where(x => x.IsIdle).ToList();
            if (idleUnitsOfWantedTypeInMyBase.Count < _minCount) return;
            ShiftAttackAllStartLocations(idleUnitsOfWantedTypeInMyBase);
        }

        private static void ShiftAttackAllStartLocations(IEnumerable<Unit> units)
        {
            var notMyBaseStartLocations = Game.StartLocations.Except(new[] { Game.Self.StartLocation })
                .Select(loc => new Position(loc.X * 32, loc.Y * 32))
                .ToList();

            units.ForEach(x => notMyBaseStartLocations.ForEach(position => x.Attack(position, true)));
        }
    }
}