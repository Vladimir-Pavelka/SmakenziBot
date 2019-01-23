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
        private readonly Subject<Unit> _unitSpawned = new Subject<Unit>();
        public IConnectableObservable<Unit> UnitSpawned { get; }

        private readonly Subject<Unit> _unitDestroyed = new Subject<Unit>();
        public IConnectableObservable<Unit> UnitDestroyed { get; }

        private readonly Subject<UnitType> _trainingStarted = new Subject<UnitType>();
        public IConnectableObservable<UnitType> TrainingStarted { get; }

        private readonly Subject<UnitType> _constructionStarted = new Subject<UnitType>();
        public IConnectableObservable<UnitType> ConstructionStarted { get; }

        private readonly AnalyzedMap _analyzedMap;
        private readonly TerrainStrategy _terrainStrategy;
        private readonly BuildOrderScheduler _buildOrderScheduler;

        private readonly IReadOnlyCollection<IBehavior> _behaviors;
        private int _frameSkip = 2;
        private int _localSpeed;

        public Bot()
        {
            //MapExporter.ExportMap();
            _analyzedMap = TerrainAnalyzerAdapter.Get();
            _terrainStrategy = new TerrainStrategy(_analyzedMap);

            UnitSpawned = _unitSpawned.Publish();
            UnitDestroyed = _unitDestroyed.Publish();
            TrainingStarted = _trainingStarted.Publish();
            ConstructionStarted = _constructionStarted.Publish();

            _buildOrderScheduler = new BuildOrderScheduler(TrainingStarted, ConstructionStarted, _terrainStrategy);

            var entranceToNaturalExp = _terrainStrategy.MyNaturals.FirstOrDefault()?.AdjacentChokes
                .Except(_terrainStrategy.ChokesBetweenMainAndNaturals).FirstOrDefault();
            var hasEntranceToNaturalExp = entranceToNaturalExp != null;
            var entranceToGuard = hasEntranceToNaturalExp
                ? entranceToNaturalExp
                : _terrainStrategy.ChokesBetweenMainAndNaturals.FirstOrDefault();

            var main = _terrainStrategy.MyStartRegion;
            _behaviors = new IBehavior[]
            {
                new IdleWorkersToMineral(main),
                new ThreeWorkersOnGas(main),
                //new CounterAttackStackWorkers(baseLocation),
                new WorkersAttackClosestEnemy(main),
                new AttackEnemiesInBase(main),
                //new StepBackIfUnderAttack(),
                new RangedKite(),
                new RememberEnemyBuildings(),
                new TowersAttackLowestHp(),
            };

            if (!_terrainStrategy.MyNaturals.Any())
            {
                _behaviors = _behaviors.Concat(new IBehavior[]
                {
                    new IdleFightersGuardEntrance(main, entranceToGuard),
                    //new OrderIdleUnitsToAttack(UnitType.Zerg_Zergling, 6, main),
                    new OrderIdleUnitsToAttack(UnitType.Zerg_Hydralisk, 20, main),

                }).ToArray();
                return;
            }

            var natural = _terrainStrategy.MyNaturals.First();
            _behaviors = _behaviors.Concat(new IBehavior[]
            {
                new IdleWorkersToMineral(natural),
                new ThreeWorkersOnGas(natural),
                new WorkersAttackClosestEnemy(natural),
                new AttackEnemiesInBase(natural),
                new IdleFightersGuardEntrance(natural, entranceToGuard),
                new FightersRallyPoint(main, natural),
                //new OrderIdleUnitsToAttack(UnitType.Zerg_Zergling, 6, natural),
                new OrderIdleUnitsToAttack(UnitType.Zerg_Hydralisk, 20, natural),
                new IdleFightersAttackClosestEnemy(natural, _terrainStrategy),
                new BalanceWorkersMainNatural(main, natural),
            }).ToArray();
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
            ProcessBuildOrder();
            ExecuteBehaviors();
        }

        private void DrawDebugInfo()
        {
            _terrainStrategy.MyNaturals.ForEach(Draw.Natural);
            //Draw.Regions(_analyzedMap.MapRegions);
            //Draw.Chokes(_analyzedMap.ChokeRegions.SelectMany(ch => ch.MinWidthWalkTilesLine));
            _analyzedMap.ChokeRegions.Select(ch => ch.MinWidthWalkTilesLine).ForEach(Draw.ChokeLine);

            MyUnits.TrackedUnits.ForEach(kvp => Game.DrawText(kvp.Key.Position, kvp.Value));

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

        private void UpdateGameSpeed()
        {
            Game.SetFrameSkip(_frameSkip);
            Game.SetLocalSpeed(_localSpeed);
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

        private void ProcessBuildOrder() => _buildOrderScheduler.OnFrame();

        private void ExecuteBehaviors() => _behaviors.ForEach(b => b.Execute());
    }
}