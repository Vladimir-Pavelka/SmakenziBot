namespace SmakenziBot
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Behaviors;
    using Behaviors.BaseBehaviors;
    using Behaviors.CombatBehaviors;
    using BroodWar.Api;
    using BroodWar.Api.Enum;
    using BuildOrder;
    using NBWTA;
    using NBWTA.Result;
    using NBWTA.Utils;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class Bot
    {
        private readonly BuildOrder.BuildOrder _buildOrder = new BuildOrder.BuildOrder();
        private readonly StepExecutor _stepExecutor;
        private static readonly Stopwatch StepRetryTimer = new Stopwatch();
        private const int RetryIntervalMs = 5000;

        private readonly Subject<Unit> _unitSpawned = new Subject<Unit>();
        public IConnectableObservable<Unit> UnitSpawned { get; }

        private readonly Subject<Unit> _unitDestroyed = new Subject<Unit>();
        public IConnectableObservable<Unit> UnitDestroyed { get; }

        private readonly Subject<UnitType> _trainingStarted = new Subject<UnitType>();
        public IConnectableObservable<UnitType> TrainingStarted { get; }

        private readonly Subject<UnitType> _constructionStarted = new Subject<UnitType>();
        public IConnectableObservable<UnitType> ConstructionStarted { get; }

        private readonly IReadOnlyCollection<HashSet<Unit>> _resourceClusters;
        private readonly IReadOnlyCollection<TilePosition> _mainBuildingLocations;

        private readonly AnalyzedMap _analyzedMap;

        private readonly IReadOnlyCollection<IBehavior> _behaviors;

        public Bot()
        {
            MapExporter.ExportMap();
            //var mapAnalyzer = new MapAnalyzer();
            //_analyzedMap = mapAnalyzer.Analyze(Game.MapWidth * 4, Game.MapHeight * 4, tile => Game.IsWalkable(tile.x, tile.y));

            UnitSpawned = _unitSpawned.Publish();
            UnitDestroyed = _unitDestroyed.Publish();
            TrainingStarted = _trainingStarted.Publish();
            ConstructionStarted = _constructionStarted.Publish();

            _stepExecutor = new StepExecutor(TrainingStarted, ConstructionStarted);

            var baseLocation = Game.Self.StartLocation;
            _behaviors = new IBehavior[]
            {
                new IdleWorkersToMineral(baseLocation),
                new ThreeWorkersOnGas(baseLocation),
                //new CounterAttackStackWorkers(baseLocation),
                new AttackEnemiesInBase(baseLocation),
                new StepBackIfUnderAttack(),
                new IdleFightersAttackClosestEnemy(), 
            };

            Game.SendText("black sheep wall");

            //var resources = Game.StaticMinerals.Concat(Game.StaticGeysers);
            //_resourceClusters = ResourceSiteLocations.Find(resources);

            //var mainBuildingPlacement = new MainBuildingPlacement(Game.MapWidth, Game.MapHeight);
            //_mainBuildingLocations = mainBuildingPlacement.CalculatePlacements(_resourceClusters, tile => Game.IsBuildable(tile, false));
        }

        public void OnGameStart()
        {
            Game.SetLocalSpeed(0);
            Game.SetFrameSkip(2);
            UnitSpawned.Connect();
            TrainingStarted.Connect();
            ConstructionStarted.Connect();

            _stepExecutor.Execute(_buildOrder.Current);
        }

        public void OnFrame()
        {
            //Draw.Regions(_analyzedMap.MapRegions);
            //Draw.Chokes(_analyzedMap.ChokeRegions);
            //Draw.ResourceClusters(_resourceClusters);
            //Draw.MainBuildingPlacements(_mainBuildingLocations);

            Game.Events.Where(x => x.Type == EventType.UnitComplete).ForEach(x => _unitSpawned.OnNext(x.Unit));
            Game.Events.Where(x => x.Type == EventType.UnitDestroy).ForEach(x => _unitDestroyed.OnNext(x.Unit));
            // TODO: how is lulker morph different?
            Game.Events.Where(x => x.Type == EventType.UnitMorph).Where(x => x.Unit.Order == OrderType.ZergUnitMorph).Where(x => x.Unit.TrainingQueue.Any()).ForEach(x => _trainingStarted.OnNext(x.Unit.TrainingQueue.First().Type));
            Game.Events.Where(x => x.Type == EventType.UnitMorph).Where(x => x.Unit.Order == OrderType.IncompleteBuilding).ForEach(x => _constructionStarted.OnNext(x.Unit.UnitType.Type));

            ExecuteBuildOrder();
            ExecuteBehaviors();
            OrderLingsToAttack();
            OrderHydrasToAttack();
        }

        private void ExecuteBuildOrder()
        {
            if (_buildOrder.Current.AllPrerequisitesMet() && (_stepExecutor.IsIdle || !_stepExecutor.IsIdle && ShouldRetry))
            {
                Game.Write($"Executing step: {_buildOrder.Current}");
                _stepExecutor.Execute(_buildOrder.Current);
                StepRetryTimer.Restart();
            }
        }

        private static bool ShouldRetry => StepRetryTimer.ElapsedMilliseconds > RetryIntervalMs;

        private void ExecuteBehaviors() => _behaviors.ForEach(b => b.Execute());

        private static void OrderLingsToAttack()
        {
            if (Game.FrameCount % 20 != 0) return;
            var zerglingsNearMyBase = UnitsNearMyBase(UnitType.Zerg_Zergling).Where(x => !x.IsAttacking).ToList();
            if (zerglingsNearMyBase.Count < 6) return;
            ShiftAttackAllStartLocations(zerglingsNearMyBase);
        }

        private static void OrderHydrasToAttack()
        {
            if ((Game.FrameCount + 1) % 20 != 0) return;
            var hydrasNearMyBase = UnitsNearMyBase(UnitType.Zerg_Hydralisk).Where(x => !x.IsAttacking).ToList();
            if (hydrasNearMyBase.Count < 12) return;
            ShiftAttackAllStartLocations(hydrasNearMyBase);
        }

        private static IEnumerable<Unit> UnitsNearMyBase(UnitType unitType) =>
            Game.Self.Units
                .Where(x => x.UnitType.Type == unitType)
                .Where(x => Game.Self.StartLocation.CalcApproximateDistance(x.TilePosition) < 20);

        private static void ShiftAttackAllStartLocations(IEnumerable<Unit> units)
        {
            var notMyBaseStartLocations = Game.StartLocations.Except(new[] { Game.Self.StartLocation })
                .Select(loc => new Position(loc.X * 32, loc.Y * 32))
                .ToList();

            units.ForEach(x => notMyBaseStartLocations.ForEach(position => x.Attack(position, true)));
        }
    }
}