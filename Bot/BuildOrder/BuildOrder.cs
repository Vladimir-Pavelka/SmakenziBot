namespace SmakenziBot.BuildOrder
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using BroodWar.Api.Enum;
    using Prerequisities;
    using Steps;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public static class Make
    {
        public static MorphUnitStep Drone => new MorphUnitStep(UnitType.Zerg_Drone, new ResourcePrerequisite(50, 0));
        public static MorphUnitStep Overlord => new MorphUnitStep(UnitType.Zerg_Overlord, new ResourcePrerequisite(100, 0));
        public static MorphUnitStep Zergling => new MorphUnitStep(UnitType.Zerg_Zergling, new ResourcePrerequisite(50, 0), new BuildingPrerequisite(UnitType.Zerg_Spawning_Pool));
        public static MorphUnitStep Hydralisk => new MorphUnitStep(UnitType.Zerg_Hydralisk, new ResourcePrerequisite(75, 25), new BuildingPrerequisite(UnitType.Zerg_Hydralisk_Den));

        public static ConstructBuildingStep Hatchery => new ConstructBuildingStep(UnitType.Zerg_Hatchery, new ResourcePrerequisite(300, 0));
        public static ConstructBuildingStep Extractor => new ConstructBuildingStep(UnitType.Zerg_Extractor, new ResourcePrerequisite(50, 0));
        public static ConstructBuildingStep SpawningPool => new ConstructBuildingStep(UnitType.Zerg_Spawning_Pool, new ResourcePrerequisite(200, 0));
        public static ConstructBuildingStep HydraliskDen => new ConstructBuildingStep(UnitType.Zerg_Hydralisk_Den, new ResourcePrerequisite(100, 50), new BuildingPrerequisite(UnitType.Zerg_Spawning_Pool));

        public static CancelConstructBuildingStep CancelExtractor => new CancelConstructBuildingStep(UnitType.Zerg_Extractor);

        public static ResearchUpgradeStep MuscularAugments => new ResearchUpgradeStep(UpgradeType.Muscular_Augments, new ResourcePrerequisite(150, 150), new BuildingPrerequisite(UnitType.Zerg_Hydralisk_Den));
        public static ResearchUpgradeStep GroovedSpines => new ResearchUpgradeStep(UpgradeType.Grooved_Spines, new ResourcePrerequisite(150, 150), new BuildingPrerequisite(UnitType.Zerg_Hydralisk_Den));
    }

    public class BuildOrder
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
            //yield return Make.Hatchery;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.MuscularAugments;
            yield return Make.Overlord;
            for (var x = 0; x < 8; x++) yield return Make.Hydralisk;
            yield return Make.Overlord;
            for (var x = 0; x < 2; x++) yield return Make.Hydralisk;
            yield return Make.GroovedSpines;

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

        private static IEnumerable<Step> PumpForever(MorphUnitStep step)
        {
            foreach (var unused in Enumerable.Range(0, int.MaxValue))
            {
                if (NeedsMoreSupply()) yield return Make.Overlord;
                else if (Game.Self.Minerals > 500) yield return Make.Hatchery;
                else if (!IsMineralLineSaturated()) yield return Make.Drone;
                else yield return step;
            }
        }

        private static bool NeedsMoreSupply() =>
            RemainingSupply < 3 && Game.Self.Units.All(u => !IsMorphTrainig(u, UnitType.Zerg_Overlord));

        private static int RemainingSupply => (Game.Self.SupplyTotal - Game.Self.SupplyUsed) / 2;

        private static bool IsMorphTrainig(Unit u, UnitType target) =>
            u.TrainingQueue.Any() && u.TrainingQueue.First().Type == target;

        private static bool IsMineralLineSaturated() => DronesInBase.Count(d => d.IsGatheringMinerals) >= 12;

        private static IEnumerable<Unit> DronesInBase =>
            Game.Self.Units.Where(u => u.UnitType.Type == UnitType.Zerg_Drone).Where(u =>
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