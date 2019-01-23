namespace SmakenziBot.BuildOrder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using Utils;
    using Steps;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class BuildOrderSteps : IBuildOrder
    {
        private readonly TerrainStrategy _terrainStrategy;

        public BuildOrderSteps(TerrainStrategy terrainStrategy)
        {
            _terrainStrategy = terrainStrategy;
        }

        public IEnumerable<Step> GetSteps()
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
            foreach (var step in Openings.NinePool()) yield return step;
            for (var x = 0; x < 3; x++) yield return Make.Zergling;
            yield return Make.Drone;
            yield return _terrainStrategy.MyNaturals.Any()
                ? Make.Hatchery(HatcheryType.NaturalExp)//_terrainStrategy.MyNaturals.First().ResourceSites.First().OptimalResourceDepotBuildTile.AsBuildTile())
                : Make.Hatchery(HatcheryType.MainMacro);
            yield return Make.Drone;
            yield return Make.Extractor;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.HydraliskDen;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.HydraSpeed;
            yield return Make.Overlord;
            for (var x = 0; x < 8; x++) yield return Make.Hydralisk;
            yield return Make.CreepColony;
            yield return Make.Overlord;
            yield return Make.SunkenColony;
            yield return Make.HydraRange;
            yield return Make.CreepColony;
            yield return Make.EvolutionChamber;
            for (var x = 0; x < 2; x++) yield return Make.Hydralisk;
            yield return Make.SunkenColony;
            yield return Make.MissileAttacks;
            for (var x = 0; x < 6; x++) yield return Make.Hydralisk;

            foreach (var step in PumpForever(() => Make.Hydralisk)) yield return step;
        }

        private static IEnumerable<Step> PumpForever(Func<Step> unitStep)
        {
            while (true)
            {
                if (NeedsMoreSupply()) yield return Make.Overlord;
                else if (NeedsMoreDrones()) yield return Make.Drone;
                else if (Game.Self.Minerals > 500 && CanMakeMacroHatch()) yield return Make.Hatchery(HatcheryType.MainMacro);
                else yield return unitStep();
            }
        }

        private static bool CanMakeMacroHatch() =>
            Game.Self.Units.Count(u => u.Is(UnitType.Zerg_Hatchery) && u.IsBeingConstructed) < 2;

        private static bool NeedsMoreSupply() =>
            RemainingSupply <= Game.Self.SupplyUsed / 2 / 10 && Game.Self.Units.All(u => !u.IsTraining(UnitType.Zerg_Overlord));

        private static int RemainingSupply => (Game.Self.SupplyTotal - Game.Self.SupplyUsed) / 2;

        private static bool NeedsMoreDrones() =>
            UnitsInBase(UnitType.Zerg_Drone).Count(d => d.IsGatheringMinerals) +
            UnitsInBase(UnitType.Zerg_Egg).Count(e => e.IsTraining(UnitType.Zerg_Drone)) < 13 && Game.Minerals.Any(IsInBase);

        private static IEnumerable<Unit> UnitsInBase(UnitType wantedType) =>
            Game.Self.Units.Where(u => u.UnitType.Type == wantedType).Where(IsInBase);

        private static bool IsInBase(Unit u) => u.TilePosition.CalcApproximateDistance(Game.Self.StartLocation) < 15;
    }
}