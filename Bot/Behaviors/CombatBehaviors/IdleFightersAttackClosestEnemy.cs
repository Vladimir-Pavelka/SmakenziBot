namespace SmakenziBot.Behaviors.CombatBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Result;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class IdleFightersAttackClosestEnemy : CombatBehavior
    {
        private readonly MapRegion _baseLocation;

        public IdleFightersAttackClosestEnemy(MapRegion baseLocation)
        {
            _baseLocation = baseLocation;
        }

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
                triggeringUnits.ForEach(u =>
                {
                    MyUnits.SetActivity(u, nameof(IdleFightersAttackClosestEnemy) + "Unit");
                    u.Attack(visibleEnemyUnits.ClosestTo(u).Position, false);
                });
                return;
            }

            if (!GameMemory.EnemyBuildings.Any()) return;

            triggeringUnits.ForEach(u =>
            {
                var closestEnemyBuilding = GameMemory.EnemyBuildings.MinBy(b => u.Position.CalcApproximateDistance(b));
                MyUnits.SetActivity(u, nameof(IdleFightersAttackClosestEnemy) + "Building");
                u.Attack(closestEnemyBuilding, false);
            });
        }

        private bool IsOutsideOfBase(Unit u) => !_baseLocation.ContentTiles.Contains(u.Position.ToWalkTile().AsTuple());
    }
}