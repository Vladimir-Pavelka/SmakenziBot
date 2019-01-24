namespace SmakenziBot.BuildOrder
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using BroodWar.Api;
    using NBWTA.Result;
    using NBWTA.Utils;
    using Steps;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class ExpandExecutor
    {
        private readonly AnalyzedMapExtra _analyzedMapExtra;
        private readonly GameInfo _gameInfo;
        private readonly IObservable<UnitType> _constructionStarted;

        public ExpandExecutor(AnalyzedMapExtra analyzedMapExtra, GameInfo gameInfo, IObservable<UnitType> constructionStarted)
        {
            _analyzedMapExtra = analyzedMapExtra;
            _gameInfo = gameInfo;
            _constructionStarted = constructionStarted;
        }

        public void Execute(HatcheryBuildingStep hatcheryStep)
        {
            var drone = MyUnits.TrackedUnits.FirstOrDefault(kvp => kvp.Value.StartsWith("MoveDroneTo")).Key ?? GetFreeDrone();
            var buildSite = GetBuildRegion(hatcheryStep);
            Construct(drone, ToBuildLocation(buildSite), hatcheryStep);
        }

        private MapRegion GetBuildRegion(HatcheryBuildingStep step)
        {
            switch (step.HatcheryType)
            {
                case HatcheryType.NaturalExp: return Natural;
                case HatcheryType.ThirdExp: return Third;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void Execute(BoAction actionStep)
        {
            var drone = GetFreeDrone();
            var expandLocation = GetBuildRegion(actionStep.ActionType);
            var isSuccess = MoveDroneToExpandLocation(drone, ToPixelPosition(expandLocation));
            if (isSuccess) actionStep.Complete();
        }

        private MapRegion GetBuildRegion(ActionType actionStepActionType)
        {
            switch (actionStepActionType)
            {
                case ActionType.MoveDroneToNatural: return Natural;
                case ActionType.MoveDroneToThird: return Third;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private static bool MoveDroneToExpandLocation(Unit drone, Position expandLocation)
        {
            MyUnits.SetActivity(drone, "MoveDroneToExpansion");
            return drone.Move(expandLocation, false);
        }

        private static Unit GetFreeDrone()
        {
            var myDrones = Game.Self.Units.Where(x => x.UnitType.Type == UnitType.Zerg_Drone).ToList();
            var freeDrones = myDrones.Where(x => x.IsIdle).Union(myDrones.Where(x => !x.IsCarryingMinerals).Where(x => !x.IsGatheringGas)).ToList();

            return freeDrones.Any() ? freeDrones.First() : myDrones.First();
        }

        private void Construct(Unit builder, TilePosition buildSite, HatcheryBuildingStep constructStep)
        {
            if (!Game.IsExplored(buildSite))
            {
                builder.Move(buildSite.ToPixelTile(), false);
                return;
            }

            builder.Build(constructStep.Target, buildSite);
            _constructionStarted.Where(x => x == constructStep.Target).Take(1).Subscribe(x =>
            {
                constructStep.Complete();
                // TODO: problem, if we start constructing another instance of the same building before, we complete the step prematurely
                _gameInfo.MyBases.Add(new MyBase(GetBuildRegion(constructStep), constructStep.HatcheryType.ToBaseType()));
            });
        }

        private MapRegion Natural => _analyzedMapExtra.MyNaturals.First();

        private MapRegion Third => _analyzedMapExtra.AllResourceRegions
            .Except(_analyzedMapExtra.MyNaturals.Concat(_analyzedMapExtra.MyStartRegion.Yield()))
            .Where(r => r.ResourceSites.Any(rs => rs.GeysersBuildTiles.Any()))
            .MinBy(r => r.ResourceSites.Min(rs => Game.Self.StartLocation.CalcApproximateDistance(rs.OptimalResourceDepotBuildTile.AsBuildTile())));

        private static TilePosition ToBuildLocation(MapRegion expansion) =>
            expansion.ResourceSites.First().OptimalResourceDepotBuildTile.AsBuildTile();

        private static Position ToPixelPosition(MapRegion expansion) => ToBuildLocation(expansion).ToPixelTile();
    }
}