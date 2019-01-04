namespace SmakenziBot.Behaviors.CombatBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class IdleFightersAttackClosestEnemy : CombatBehavior
    {
        public override void Execute()
        {
            MyCombatUnits.Where(u => u.IsIdle).Where(IsOutsideOfBase).ForEach(u =>
            {
                if (Game.Enemy.Units.All(eu => eu.UnitType.Type == UnitType.Unknown))
                {
                    if (!GameMemory.EnemyBuildings.Any()) return;
                    var closestEnemyBuilding = GameMemory.EnemyBuildings
                        .MinBy(b => u.Position.CalcApproximateDistance(b));
                    u.Attack(closestEnemyBuilding, false);
                    return;
                }

                var closestEnemyUnit = Game.Enemy.Units.ClosestTo(u);
                u.Attack(closestEnemyUnit.Position, false);
            });
        }

        private static bool IsOutsideOfBase(Unit u) => u.TilePosition.CalcApproximateDistance(Game.Self.StartLocation) > 15;
    }
}