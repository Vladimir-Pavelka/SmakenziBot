namespace SmakenziBot.BuildOrder
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using BroodWar.Api;
    using Steps;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class StepExecutor
    {
        private static readonly Random Rnd = new Random();

        private readonly IObservable<UnitType> _trainingStarted;
        private readonly IObservable<UnitType> _constructionStarted;

        public StepExecutor(IObservable<UnitType> trainingStarted, IObservable<UnitType> constructionStarted)
        {
            _trainingStarted = trainingStarted;
            _constructionStarted = constructionStarted;
        }

        public void Execute(Step step)
        {
            IsIdle = false;
            if (step is MorphUnitStep morphStep)
            {
                Morph(morphStep.Item);
                _trainingStarted.Where(x => x == morphStep.Item).Take(1).Subscribe(x => CompleteStep(step));
                return;
            }

            if (step is ConstructBuildingStep constructStep)
            {
                var builder = GetFreeDrone();
                var buildSite = FindBuildSite(builder, constructStep.Item);
                builder.Build(constructStep.Item, buildSite);
                _constructionStarted.Where(x => x == constructStep.Item).Take(1).Subscribe(x => CompleteStep(step));
            }

            if (step is ResearchUpgradeStep researchStep)
            {
                var researcher = Game.Self.Units.Where(x => x.UnitType.Type == researchStep.ResearchedBy).First(x => !x.IsResearching);
                var isSuccess = researcher.PerformUpgrade(researchStep.Research);
                if (isSuccess) CompleteStep(step);
            }

            if (step is CancelConstructBuildingStep cancelStep)
            {
                var buildingToCancel = Game.Self.Units.Where(x => x.UnitType.Type == cancelStep.Item).First(x => x.IsBeingConstructed);
                var isSuccess = buildingToCancel.CancelConstruction();
                if (isSuccess) CompleteStep(step);
            }
        }

        private void CompleteStep(Step step)
        {
            IsIdle = true;
            step.Complete();
        }

        private static TilePosition FindBuildSite(Unit builder, UnitType building)
        {
            var basePosition = Game.Self.StartLocation;

            if (building == UnitType.Zerg_Extractor)
            {
                var geyser = Game.Geysers.OrderBy(x => basePosition.CalcApproximateDistance(x.TilePosition)).First();
                return geyser.TilePosition;
            }

            var buildSite = Enumerable.Range(-20, 40).Select(x => basePosition.X + x + (x > 0 ? 2 : -2))
                .SelectMany(x => Enumerable.Range(-20, 40).Select(y => basePosition.Y + y + (y > 0 ? 2 : -2)).Select(y => new TilePosition(x, y)))
                .OrderBy(site => basePosition.CalcApproximateDistance(site))
                .Where(site => Game.CanBuildHere(site, building, builder, true))
                .Skip(Rnd.Next(5))
                .First();

            if (buildSite == null) throw new Exception("Could not find suitable build site");
            return buildSite;
        }

        private static Unit GetFreeDrone()
        {
            var myDrones = Game.Self.Units.Where(x => x.UnitType.Type == UnitType.Zerg_Drone).ToList();
            var freeDrones = myDrones.Where(x => !x.IsCarryingMinerals).Where(x => !x.IsGatheringGas).ToList();

            return freeDrones.Any() ? freeDrones.First() : myDrones.First();
        }

        public bool IsIdle { get; private set; }

        //private static bool IsMorphable(UnitType unitType) => new[]
        //        {UnitType.Zerg_Drone, UnitType.Zerg_Overlord, UnitType.Zerg_Zergling, UnitType.Zerg_Hydralisk}
        //    .Contains(unitType);

        //private static bool IsBuildable(UnitType unitType) => new[]
        //        {UnitType.Zerg_Hatchery, UnitType.Zerg_Extractor, UnitType.Zerg_Spawning_Pool, UnitType.Zerg_Hydralisk_Den}
        //    .Contains(unitType);

        private static void Morph(UnitType unitType)
        {
            var larva = Game.Self.Units.First(x => x.UnitType.Type == UnitType.Zerg_Larva);
            larva.Train(unitType);
        }
    }
}