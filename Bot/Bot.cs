namespace SmakenziBot
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Behaviors;
    using Behaviors.BaseBehaviors;
    using Behaviors.CombatBehaviors;
    using Behaviors.GameBehaviors;
    using BroodWar.Api;
    using BroodWar.Api.Enum;
    using BuildOrder;
    using NBWTA.Result;
    using NBWTA.Utils;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class Bot
    {
        private readonly BuildOrderSteps _buildOrderSteps = new BuildOrderSteps();
        private readonly StepExecutor _stepExecutor;
        private static int _buildOrderLastStepFrame;
        private const int RetryIntervalFrames = 200;

        private readonly Subject<Unit> _unitSpawned = new Subject<Unit>();
        public IConnectableObservable<Unit> UnitSpawned { get; }

        private readonly Subject<Unit> _unitDestroyed = new Subject<Unit>();
        public IConnectableObservable<Unit> UnitDestroyed { get; }

        private readonly Subject<UnitType> _trainingStarted = new Subject<UnitType>();
        public IConnectableObservable<UnitType> TrainingStarted { get; }

        private readonly Subject<UnitType> _constructionStarted = new Subject<UnitType>();
        public IConnectableObservable<UnitType> ConstructionStarted { get; }

        private readonly AnalyzedMap _analyzedMap;

        private readonly IReadOnlyCollection<IBehavior> _behaviors;
        private int _frameSkip = 2;
        private int _localSpeed;

        public Bot()
        {
            //MapExporter.ExportMap();
            _analyzedMap = TerrainAnalyzerAdapter.Get();

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
                new WorkersAttackClosestEnemy(baseLocation),
                new AttackEnemiesInBase(baseLocation),
                //new StepBackIfUnderAttack(),
                new RangedKite(),
                new IdleFightersAttackClosestEnemy(),
                new OrderIdleUnitsToAttackSpawnLocations(UnitType.Zerg_Zergling, 6, baseLocation),
                new OrderIdleUnitsToAttackSpawnLocations(UnitType.Zerg_Hydralisk, 12, baseLocation),
                new RememberEnemyBuildings(), 
            };
        }

        public void OnGameStart()
        {
            //Game.SendText("black sheep wall");

            Game.EnableFlag(Flag.UserInput);
            Game.SetLocalSpeed(_localSpeed);
            Game.SetFrameSkip(_frameSkip);
            Game.SetCommandOptimizationLevel(1);
            UnitSpawned.Connect();
            TrainingStarted.Connect();
            ConstructionStarted.Connect();
        }

        public void OnFrame()
        {
            DrawDebugInfo();
            ProcessUserInput();
            UpdateEventStreams();
            ExecuteBuildOrder();
            ExecuteBehaviors();
        }

        private void DrawDebugInfo()
        {
            //Draw.Regions(_analyzedMap.MapRegions);
            //Draw.Chokes(_analyzedMap.ChokeRegions.SelectMany(ch => ch.MinWidthWalkTilesLine));
            _analyzedMap.ChokeRegions.Select(ch => ch.MinWidthWalkTilesLine).ForEach(Draw.ChokeLine);

            var resourceClusters = _analyzedMap.MapRegions.SelectMany(r => r.ResourceSites)
                .Select(s => s.MineralsBuildTiles.Concat(s.GeysersBuildTiles));
            Draw.ResourceClusters(resourceClusters);

            var allResourceDepotPositions =
                _analyzedMap.MapRegions.SelectMany(r => r.ResourceSites.Select(s => s.OptimalResourceDepotBuildTile));

            Draw.MainBuildingPlacements(allResourceDepotPositions);
        }

        private void ProcessUserInput()
        {
            if (Game.GetKeyState(Key.Q))
            {
                if (_frameSkip > 1) _frameSkip--;
                else _localSpeed += 5;
                UpdateGameSpeed();
            }

            if (Game.GetKeyState(Key.W))
            {
                if (_localSpeed > 0) _localSpeed -= 5;
                else _frameSkip++;
                UpdateGameSpeed();
            }
        }

        private void UpdateEventStreams()
        {
            Game.Events.Where(x => x.Type == EventType.UnitComplete).ForEach(x => _unitSpawned.OnNext(x.Unit));
            Game.Events.Where(x => x.Type == EventType.UnitDestroy).ForEach(x => _unitDestroyed.OnNext(x.Unit));
            // TODO: how is lulker morph different?
            Game.Events.Where(x => x.Type == EventType.UnitMorph).Where(x => x.Unit.Order == OrderType.ZergUnitMorph)
                .Where(x => x.Unit.TrainingQueue.Any())
                .ForEach(x => _trainingStarted.OnNext(x.Unit.TrainingQueue.First().Type));
            Game.Events.Where(x => x.Type == EventType.UnitMorph).Where(x => x.Unit.Order == OrderType.IncompleteBuilding)
                .ForEach(x => _constructionStarted.OnNext(x.Unit.UnitType.Type));
        }

        private void ExecuteBuildOrder()
        {
            if (!_buildOrderSteps.Current.AllPrerequisitesMet()) return;
            if (!_stepExecutor.IsIdle && !ShouldRetry) return;

            Game.Write($"Executing step: {_buildOrderSteps.Current}");
            _stepExecutor.Execute(_buildOrderSteps.Current);
            _buildOrderLastStepFrame = Game.FrameCount;
        }

        private static bool ShouldRetry => Game.FrameCount - _buildOrderLastStepFrame > RetryIntervalFrames;
        private void ExecuteBehaviors() => _behaviors.ForEach(b => b.Execute());

        private void UpdateGameSpeed()
        {
            Game.SetFrameSkip(_frameSkip);
            Game.SetLocalSpeed(_localSpeed);
        }
    }
}