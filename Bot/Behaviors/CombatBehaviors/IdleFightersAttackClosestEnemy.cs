namespace SmakenziBot.Behaviors.CombatBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;

    public class IdleFightersAttackClosestEnemy : CombatBehavior
    {
        public override void Execute()
        {
            MyCombatUnits.Where(IsOutsideOfBase).Where(u => u.IsIdle).ForEach(u =>
            {
                if (Game.Enemy.Units.Any()) u.Attack(Game.Enemy.Units.ClosestTo(u), false);
            });
        }

        private static bool IsOutsideOfBase(Unit u) => u.TilePosition.CalcApproximateDistance(Game.Self.StartLocation) > 15;
    }
}