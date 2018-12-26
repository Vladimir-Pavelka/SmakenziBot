namespace SmakenziBot.Behaviors.CombatBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;
    using Utils;

    public class IdleFightersAttackClosestEnemy : CombatBehavior
    {
        public override void Execute()
        {
            MyCombatUnits.Where(u => u.IsIdle).Where(IsOutsideOfBase).ForEach(u =>
            {
                if (Game.Enemy.Units.Any()) u.Attack(Game.Enemy.Units.ClosestTo(u).Position, false);
            });
        }

        private static bool IsOutsideOfBase(Unit u) => u.TilePosition.CalcApproximateDistance(Game.Self.StartLocation) > 15;
    }
}