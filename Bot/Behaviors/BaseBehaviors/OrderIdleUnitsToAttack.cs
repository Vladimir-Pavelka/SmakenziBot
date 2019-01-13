namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class OrderIdleUnitsToAttack : BaseBehavior
    {
        private readonly UnitType _unitType;
        private readonly int _minCount;

        public OrderIdleUnitsToAttack(UnitType unitType, int minCount, TilePosition basePosition) : base(basePosition)
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
            if (GameMemory.EnemyBuildings.Any())
            {
                AttackClosestEnemyBuilding(idleUnitsOfWantedTypeInMyBase);
                return;
            }

            ShiftAttackAllStartLocations(idleUnitsOfWantedTypeInMyBase);
        }

        private static void AttackClosestEnemyBuilding(IReadOnlyCollection<Unit> idleUnitsOfWantedTypeInMyBase)
        {
            var closestEnemyBuilding = GameMemory.EnemyBuildings.MinBy(b =>
                b.CalcApproximateDistance(idleUnitsOfWantedTypeInMyBase.First().Position));

            idleUnitsOfWantedTypeInMyBase.ForEach(u => u.Attack(closestEnemyBuilding, false));
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