namespace SmakenziBot.BuildOrder
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using Utils;
    using Steps;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class BuildOrderSteps
    {
        private static IEnumerable<Step> GetSteps()
        {
            //for (var x = 0; x < 5; x++) yield return Make.Drone;
            //yield return Make.Overlord;
            //for (var x = 0; x < 3; x++) yield return Make.Drone;
            //yield return Make.Hatchery;
            //yield return Make.SpawningPool;
            //for (var x = 0; x < 3; x++) yield return Make.Drone;
            //yield return Make.Hatchery;
            //for (var x = 0; x < 3; x++) yield return Make.Zergling;
            //yield return Make.Overlord;
            //for (var x = 0; x < 5; x++) yield return Make.Zergling;
            //yield return Make.Overlord;
            //for (var x = 0; x < 18; x++) yield return Make.Zergling;
            //yield return Make.Overlord;
            //foreach (var unused in Enumerable.Range(0, int.MaxValue)) yield return Make.Zergling;
            foreach (var step in NinePool()) yield return step;
            for (var x = 0; x < 3; x++) yield return Make.Zergling;
            yield return Make.Drone;
            yield return Make.Hatchery;
            yield return Make.Drone;
            yield return Make.Extractor;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.HydraliskDen;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.MuscularAugments;
            yield return Make.Overlord;
            for (var x = 0; x < 8; x++) yield return Make.Hydralisk;
            yield return Make.CreepColony;
            yield return Make.Overlord;
            yield return Make.SunkenColony;
            yield return Make.GroovedSpines;
            yield return Make.CreepColony;
            yield return Make.EvolutionChamber;
            for (var x = 0; x < 2; x++) yield return Make.Hydralisk;
            yield return Make.SunkenColony;
            yield return Make.MissileAttacks;

            foreach (var step in PumpForever(Make.Hydralisk)) yield return step;
        }

        public static IEnumerable<Step> NinePool()
        {
            for (var x = 0; x < 5; x++) yield return Make.Drone;
            yield return Make.SpawningPool;
            yield return Make.Drone;
            yield return Make.Extractor;
            yield return Make.Drone;
            yield return Make.CancelExtractor;
            yield return Make.Overlord;
        }

        private static IEnumerable<Step> PumpForever(Step unitStep)
        {
            while (true)
            {
                if (NeedsMoreSupply()) yield return Make.Overlord;
                else if (NeedsMoreDrones()) yield return Make.Drone;
                else if (Game.Self.Minerals > 500) yield return Make.Hatchery;
                else yield return unitStep;
            }
        }

        private static bool NeedsMoreSupply() =>
            RemainingSupply < 3 && Game.Self.Units.All(u => !u.IsTraining(UnitType.Zerg_Overlord));

        private static int RemainingSupply => (Game.Self.SupplyTotal - Game.Self.SupplyUsed) / 2;

        private static bool NeedsMoreDrones() =>
            UnitsInBase(UnitType.Zerg_Drone).Count(d => d.IsGatheringMinerals) +
            UnitsInBase(UnitType.Zerg_Egg).Count(e => e.IsTraining(UnitType.Zerg_Drone)) < 13;

        private static IEnumerable<Unit> UnitsInBase(UnitType wantedType) =>
            Game.Self.Units.Where(u => u.UnitType.Type == wantedType).Where(u =>
                u.TilePosition.CalcApproximateDistance(Game.Self.StartLocation) < 15);

        private static readonly IEnumerator<Step> StepEnumerator = GetSteps().GetEnumerator();

        public Step Current
        {
            get
            {
                while (StepEnumerator.Current == null) StepEnumerator.MoveNext();
                if (StepEnumerator.Current.IsCompleted) StepEnumerator.MoveNext();
                return StepEnumerator.Current;
            }
        }
    }
}