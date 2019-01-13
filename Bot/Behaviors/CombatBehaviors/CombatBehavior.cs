namespace SmakenziBot.Behaviors.CombatBehaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using Utils;

    public abstract class CombatBehavior : IBehavior
    {
        public abstract void Execute();

        protected IEnumerable<Unit> MyCombatUnits => Game.Self.Units.Where(u => u.IsFighter());

        protected Unit GetClosestEnemyAttacker(Unit origin) => Game.Enemy.Units.Where(u => u.UnitType.CanAttack)
            .MinBy(u => u.TilePosition.CalcApproximateDistance(origin.TilePosition));

        protected static (double X, double Y) GetRetreatVector(Unit attacker, Unit defender)
        {
            var attackerToDefenderVector = defender.Position - attacker.Position;
            var vectorLength = attackerToDefenderVector.CalcLength();

            var unitLengthVector =
                (attackerToDefenderVector.X / vectorLength,
                attackerToDefenderVector.Y / vectorLength);

            return unitLengthVector;
        }
    }
}