namespace SmakenziBot.Behaviors.CombatBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;

    public class StepBackIfUnderAttack : CombatBehavior
    {
        public override void Execute()
        {
            MyCombatUnits.Where(u => u.IsUnderAttack)
                .Where(u => u.UnitsInRadius(60).Where(Game.Self.Units.Contains).Count() < 3)
                .Select(u => (defender: u, attacker: GetClosestEnemyFighter(u)))
                .Select(pair => (defender: pair.defender, stepBackTo: GetRetreatVector(pair.attacker, pair.defender)))
                .ForEach(pair => pair.defender.Move(pair.stepBackTo, false));
        }
    }
}