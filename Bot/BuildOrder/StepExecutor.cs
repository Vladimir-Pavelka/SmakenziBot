namespace SmakenziBot.BuildOrder
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using BroodWar.Api;
    using BroodWar.Api.Enum;
    using Steps;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class StepExecutor
    {
        private static readonly Random Rnd = new Random();

        private readonly IObservable<UnitType> _trainingStarted;
        private readonly IObservable<UnitType> _constructionStarted;
        private readonly TerrainStrategy _terrainStrategy;

        public StepExecutor(IObservable<UnitType> trainingStarted, IObservable<UnitType> constructionStarted, TerrainStrategy terrainStrategy)
        {
            _trainingStarted = trainingStarted;
            _constructionStarted = constructionStarted;
            _terrainStrategy = terrainStrategy;
        }

        public void Execute(Step step)
        {
            switch (step)
            {
                case MorphUnitStep morphStep:
                    MorphLarvaInto(morphStep.Target);
                    _trainingStarted.Where(x => x == morphStep.Target).Take(1).Subscribe(x => CompleteStep(step));
                    return;
                case ConstructBuildingStep constructStep:
                    {
                        var builder = GetFreeDrone();
                        var buildSite = FindBuildSite(builder, constructStep);
                        if (!Game.IsExplored(buildSite))
                        {
                            builder.Move(buildSite.ToPixelTile(), false);
                            return;
                        }

                        builder.Build(constructStep.Target, buildSite);
                        _constructionStarted.Where(x => x == constructStep.Target).Take(1).Subscribe(x => CompleteStep(step));
                        return;
                    }
                case ResearchUpgradeStep researchStep:
                    {
                        var researcher = Game.Self.Units.Where(x => x.Is(researchStep.ResearchedBy)).First(x => !x.IsUpgrading);
                        var isSuccess = researcher.PerformUpgrade(researchStep.Research);
                        if (isSuccess) CompleteStep(step);
                        return;
                    }
                case ResearchTechStep researchStep:
                    {
                        var researcher = Game.Self.Units.Where(x => x.Is(researchStep.ResearchedBy)).First(x => !x.IsResearching);
                        var isSuccess = researcher.Research(researchStep.Research);
                        if (isSuccess) CompleteStep(step);
                        return;
                    }
                case CancelConstructBuildingStep cancelStep:
                    {
                        var buildingToCancel = Game.Self.Units.Where(x => x.UnitType.Type == cancelStep.Target).First(x => x.IsBeingConstructed);
                        var isSuccess = buildingToCancel.CancelConstruction();
                        if (isSuccess) CompleteStep(step);
                        return;
                    }
                case MorphUpgradeBuildingStep upgradeBuildingStep:
                    {
                        var buildingToUpgrade = Game.Self.Units
                            .Where(x => x.UnitType.Type == upgradeBuildingStep.WhatMorphs)
                            .First(x => !x.IsBeingConstructed && !x.IsResearching);

                        var isSuccess = buildingToUpgrade.Morph(upgradeBuildingStep.Target);
                        if (isSuccess) CompleteStep(step);
                        return;
                    }
                case BoAction actionStep:
                    {
                        var drone = GetFreeDrone();
                        bool isSuccess;

                        switch (actionStep.ActionType)
                        {
                            case ActionType.MoveDroneToNatural:
                                MyUnits.SetActivity(drone, nameof(ActionType.MoveDroneToNatural));
                                isSuccess = drone.Move(NaturalBuildLocation.ToPixelTile(), false);
                                if (isSuccess) CompleteStep(step);
                                return;
                            case ActionType.MoveDroneToThird:
                                MyUnits.SetActivity(drone, nameof(ActionType.MoveDroneToThird));
                                isSuccess = drone.Move(ThirdBuildLocation.ToPixelTile(), false);
                                if (isSuccess) CompleteStep(step);
                                return;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
            }
        }

        private void CompleteStep(Step step)
        {
            step.Complete();
        }

        private TilePosition FindBuildSite(Unit builder, ConstructBuildingStep constructStep)
        {
            var building = constructStep.Target;
            var basePosition = Game.Self.StartLocation;
            var buildLocation = Game.GetBuildLocation(UnitTypes.All[building], basePosition, 64, false);

            if (building == UnitType.Zerg_Creep_Colony) buildLocation = CreepColonyNearChoke();
            if (constructStep is HatcheryBuildingStep hatcheryStep && hatcheryStep.HatcheryType == HatcheryType.NaturalExp)
                buildLocation = NaturalBuildLocation;

            if (constructStep is HatcheryBuildingStep hatcheryStepp && hatcheryStepp.HatcheryType == HatcheryType.ThirdExp)
                buildLocation = ThirdBuildLocation;

            if (buildLocation == null) throw new Exception("Could not find suitable build site");
            return buildLocation;
        }

        private static Unit GetFreeDrone()
        {
            var myDrones = Game.Self.Units.Where(x => x.UnitType.Type == UnitType.Zerg_Drone).ToList();
            var freeDrones = myDrones.Where(x => x.IsIdle).Union(myDrones.Where(x => !x.IsCarryingMinerals).Where(x => !x.IsGatheringGas)).ToList();

            return freeDrones.Any() ? freeDrones.First() : myDrones.First();
        }

        private TilePosition NaturalBuildLocation => _terrainStrategy.MyNaturals.First().ResourceSites.First()
            .OptimalResourceDepotBuildTile.AsBuildTile();

        private TilePosition ThirdBuildLocation => _terrainStrategy.AllResourceSites
            .Except(_terrainStrategy.MyNaturals.SelectMany(n => n.ResourceSites)
                .Concat(_terrainStrategy.MyStartRegion.ResourceSites))
            .Where(rs => rs.GeysersBuildTiles.Any())
            .MinBy(rs => Game.Self.StartLocation.CalcApproximateDistance(rs.OptimalResourceDepotBuildTile.AsBuildTile()))
            .OptimalResourceDepotBuildTile.AsBuildTile();

        private static void MorphLarvaInto(UnitType unitType)
        {
            var larva = Game.Self.Units.First(x => x.UnitType.Type == UnitType.Zerg_Larva);
            larva.Morph(unitType);
        }

        private TilePosition CreepColonyNearChoke()
        {
            var naturalChoke = _terrainStrategy.MyNaturals.First().AdjacentChokes.Except(_terrainStrategy.ChokesBetweenMainAndNaturals).First();
            var chokePosition = naturalChoke.ContentTiles.First().AsWalkTile().ToBuildTile();
            var buildLocation = Enumerable.Range(-20, 40).Select(x => chokePosition.X + x + (x > 0 ? 2 : 0))
                .SelectMany(x => Enumerable.Range(-20, 40).Select(y => chokePosition.Y + y + (y > 0 ? 2 : 0)).Select(y => new TilePosition(x, y)))
                .OrderBy(naturalChoke.ContentTiles.First().AsWalkTile().ToBuildTile().CalcApproximateDistance)
                .Where(site => Game.CanBuildHere(site, UnitType.Zerg_Creep_Colony, null, true))
                .Skip(Rnd.Next(10))
                .First();

            return buildLocation;
        }
    }
}