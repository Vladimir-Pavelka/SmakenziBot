namespace SmakenziBot
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using BroodWar.Api;
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
            if (IsMorphable(step.Item))
            {
                Morph(step.Item);
                _trainingStarted.Where(x => x == step.Item).Take(1).Subscribe(x => { IsIdle = true; step.Complete(); });
                return;
            }

            if (IsBuildable(step.Item))
            {
                var builder = GetFreeDrone();
                var buildSite = FindBuildSite(builder, step.Item);
                builder.Build(step.Item, buildSite);
                _constructionStarted.Where(x => x == step.Item).Take(1).Subscribe(x => { IsIdle = true; step.Complete(); });
            }
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
            var freeDrones = myDrones.Where(x => !x.IsCarryingMinerals).ToList();

            return freeDrones.Any() ? freeDrones.First() : myDrones.First();
        }

        public bool IsIdle { get; private set; }

        private static bool IsMorphable(UnitType unitType) => new[]
                {UnitType.Zerg_Drone, UnitType.Zerg_Overlord, UnitType.Zerg_Zergling}
            .Contains(unitType);

        private static bool IsBuildable(UnitType unitType) => new[]
                {UnitType.Zerg_Hatchery, UnitType.Zerg_Spawning_Pool}
            .Contains(unitType);

        private static void Morph(UnitType unitType)
        {
            var larva = Game.Self.Units.First(x => x.UnitType.Type == UnitType.Zerg_Larva);
            larva.Train(unitType);
        }
    }
}