namespace SmakenziBot.Behaviors.CombatBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class IdleFightersAttackClosestEnemy : CombatBehavior
    {
        public override void Execute()
        {
            var triggeringUnits = MyCombatUnits.Where(u => u.IsIdle).Where(IsOutsideOfBase).ToList();
            if (!triggeringUnits.Any()) return;

            var visibleEnemyUnits = Game.Enemy.Units
                .Where(eu => eu.UnitType.Type != UnitType.Unknown)
                .Where(eu => !eu.IsCloaked)
                .ToList();

            if (visibleEnemyUnits.Any())
            {
                triggeringUnits.ForEach(u => u.Attack(visibleEnemyUnits.ClosestTo(u).Position, false));
                return;
            }

            if (!GameMemory.EnemyBuildings.Any()) return;

            triggeringUnits.ForEach(u =>
            {
                var closestEnemyBuilding = GameMemory.EnemyBuildings.MinBy(b => u.Position.CalcApproximateDistance(b));
                u.Attack(closestEnemyBuilding, false);
            });
        }

        private static bool IsOutsideOfBase(Unit u) => u.TilePosition.CalcApproximateDistance(Game.Self.StartLocation) > 15;
    }
}