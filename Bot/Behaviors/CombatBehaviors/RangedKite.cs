namespace SmakenziBot.Behaviors.CombatBehaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class RangedKite : CombatBehavior
    {
        public override void Execute()
        {
            if (Game.FrameCount % 5 != 0) return;
            var ownGroundRangedUnits = Game.Self.Units
                .Where(u => u.UnitType.Type == UnitType.Zerg_Hydralisk);

            foreach (var attacker in ownGroundRangedUnits)
            {
                var candidateTargets = EnemyUnitsInSight(attacker);
                if (!candidateTargets.Any()) continue;
                var target = GetHighestPrioTarget(attacker, candidateTargets);

                if (ShouldKite(attacker, target)) Kite(attacker, target);
                else attacker.Attack(target, false);
            }
        }

        private static IReadOnlyCollection<Unit> EnemyUnitsInSight(Unit unit)
        {
            var unitsSightRange = Game.Self.SightRange(unit.UnitType.Type) + 32;
            return unit.UnitsInRadius(unitsSightRange)
                .Where(Game.Enemy.Units.Contains)
                .Where(u => !u.IsCloaked)
                .ToList();
        }

        private Unit GetHighestPrioTarget(Unit attacker, IEnumerable<Unit> candidateTargets) =>
            candidateTargets.GroupBy(GetAttackPriority).MaxBy(x => x.Key).ClosestTo(attacker);

        private static int GetAttackPriority(Unit candidateTarget)
        {
            if (!candidateTarget.IsDetected) return 0;
            var unitType = candidateTarget.UnitType.Type;
            if (HighAttackPriorityUnits.TryGetValue(unitType, out var priority)) return priority;
            if (unitType == UnitType.Protoss_Photon_Cannon || unitType == UnitType.Zerg_Sunken_Colony) return 55;
            if (candidateTarget.IsFighter()) return 50;
            if (unitType == UnitType.Terran_Bunker) return 45;
            if (candidateTarget.UnitType.IsWorker) return 40;
            if (candidateTarget.UnitType.Price.Gas > 0) return 35;
            return 10;
        }

        private static readonly IDictionary<UnitType, int> HighAttackPriorityUnits = new Dictionary<UnitType, int>
        {
            {UnitType.Protoss_Arbiter, 100},
            {UnitType.Protoss_High_Templar, 100},
            {UnitType.Protoss_Dark_Templar, 100},
            {UnitType.Protoss_Reaver, 100},

            {UnitType.Protoss_Observer, 95},
            {UnitType.Protoss_Shuttle, 95},

            {UnitType.Protoss_Carrier, 90},
        };

        private static bool ShouldKite(Unit attacker, Unit target)
        {
            if (target.GroundRangePx() >= attacker.SelfGroundRangePx()) return false;
            if (target.UnitType.IsBuilding) return false;
            if (target.UnitType.IsWorker) return false;
            if (CanAttackNow(attacker)) return false;
            if (TimeToReachShootingRange(attacker, target) >= attacker.GroundWeaponCooldown) return false;
            if (!IsTargetFacingTowardsAttacker(attacker, target)) return false;

            return true;
        }

        private static bool IsTargetFacingTowardsAttacker(Unit attacker, Unit target)
        {
            const double twoPi = 2 * Math.PI;
            var deltaX = target.Position.X - attacker.Position.X;
            var deltaY = attacker.Position.Y - target.Position.Y;
            var attackerToTargetRads = Math.Atan2(deltaY, deltaX);
            var attackerToTargetBwapiRads = ToBwapiRads(attackerToTargetRads);
            var targetToAttackerBwapiRads = (attackerToTargetBwapiRads + Math.PI) % twoPi;
            var coneStart = Mod(targetToAttackerBwapiRads - Math.PI / 4, twoPi);
            var coneEnd = Mod(targetToAttackerBwapiRads + Math.PI / 4, twoPi);

            if (coneStart > coneEnd)
                return coneStart <= target.Angle || target.Angle <= coneEnd;
            return coneStart <= target.Angle && target.Angle <= coneEnd;
        }

        public static double ToBwapiRads(double csharpRads) =>
            csharpRads > 0 ? 2 * Math.PI - csharpRads : csharpRads < 0 ? csharpRads * -1 : 0;

        private static double Mod(double x, double m) => (x % m + m) % m;

        private static bool CanAttackNow(Unit u) => u.GroundWeaponCooldown == 0;

        private static double TimeToReachShootingRange(Unit attacker, Unit target)
        {
            var attackRange = attacker.SelfGroundRangePx();
            var moveSpeed = attacker.SelfTopSpeed();
            var distanceToTarget = attacker.Distance(target);
            return Math.Max((distanceToTarget - attackRange) / moveSpeed, 0);
        }

        private static void Kite(Unit kiter, Unit target)
        {
            const int extraDistance = 32;
            var retreatVector = GetRetreatVector(target, kiter);
            var wantedDistance = kiter.SelfGroundRangePx() + extraDistance;
            var fleeToX = Round(retreatVector.X * wantedDistance + target.Position.X);
            var fleeToY = Round(retreatVector.Y * wantedDistance + target.Position.Y);
            var fleeTo = new Position(fleeToX, fleeToY);
            kiter.Move(fleeTo, false);
        }

        private static int Round(double x) => (int)Math.Round(x);
    }
}