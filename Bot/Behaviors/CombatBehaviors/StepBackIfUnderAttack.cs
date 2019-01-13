namespace SmakenziBot.Behaviors.CombatBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;

    public class StepBackIfUnderAttack : CombatBehavior
    {
        public override void Execute()
        {
            //MyCombatUnits.Where(u => u.IsUnderAttack)
            //    .Select(u => (defender: u, nearbyUnits: u.UnitsInRadius(60)))
            //    .Where(pair => pair.nearbyUnits.Where(Game.Enemy.Units.Contains).Count() > 1)
            //    .Select(pair => (defender: pair.defender, alliedUnits: pair.nearbyUnits.Where(Game.Self.Units.Contains), attacker: GetClosestEnemyAttacker(pair.defender)))
            //    .Select(pair => (defender: pair.defender, alliedUnits: pair.alliedUnits, stepBackTo: GetRetreatVector(pair.attacker, pair.defender))) // unit vector
            //    .ForEach(pair => pair.alliedUnits.Where(u => !CanAttackNow(u)).ForEach(u => u.Move(, false)));
        }

        private static bool CanAttackNow(Unit u) => u.GroundWeaponCooldown == 0;
    }
}