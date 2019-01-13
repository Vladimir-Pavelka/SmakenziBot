namespace SmakenziBot.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public static class UnitExtensions
    {
        public static Unit ClosestTo(this IEnumerable<Unit> units, Unit target) => units.ClosestTo(target.TilePosition);

        public static Unit ClosestTo(this IEnumerable<Unit> units, TilePosition targetPosition) =>
            units.MinBy(u => u.TilePosition.CalcApproximateDistance(targetPosition));

        public static TilePosition AveragePosition(this IReadOnlyCollection<Unit> units)
        {
            var avgX = (int)Math.Round(units.Average(w => w.TilePosition.X));
            var avgY = (int)Math.Round(units.Average(w => w.TilePosition.Y));
            return new TilePosition(avgX, avgY);
        }

        public static bool IsFighter(this Unit unit) => unit.UnitType.CanAttack && !unit.UnitType.IsWorker;

        public static bool IsTraining(this Unit unit, UnitType wantedType) =>
            unit.TrainingQueue.Any() && unit.TrainingQueue.First().Type == wantedType;

        public static int GroundRangePx(this Unit unit) => unit.UnitType.GroundWeapon.MaxRange;
        public static int SelfGroundRangePx(this Unit unit) => Game.Self.WeaponMaxRange(unit.UnitType.GroundWeapon);
        public static double SelfTopSpeed(this Unit unit) => Game.Self.TopSpeed(unit.UnitType.Type);
        public static bool Is(this Unit unit, UnitType unitType) => unit.UnitType.Type == unitType;
    }
}