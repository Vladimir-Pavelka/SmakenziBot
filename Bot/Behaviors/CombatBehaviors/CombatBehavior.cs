namespace SmakenziBot.Behaviors.CombatBehaviors
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using Utils;

    public abstract class CombatBehavior : IBehavior
    {
        public abstract void Execute();

        protected IEnumerable<Unit> MyCombatUnits => Game.Self.Units.Where(u => u.IsFighter());

        protected Unit GetClosestEnemyFighter(Unit origin) => Game.Enemy.Units.Where(u => u.IsFighter())
            .OrderBy(u => u.TilePosition.CalcApproximateDistance(origin.TilePosition)).First();

        protected Position GetRetreatVector(Unit attacker, Unit defender) =>
            defender.Position + defender.Position - attacker.Position;
    }
}