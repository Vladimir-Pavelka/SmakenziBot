namespace SmakenziBot.BuildOrder.ZvT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;
    using Steps;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class BuildOrderZvT : IBuildOrder
    {
        public IEnumerable<Step> GetSteps()
        {
            return Openings.TwelveHatch()
                .Concat(new Step[]
                {
                    Make.Drone,
                    Make.Drone,
                    Make.Drone,
                    Make.Hatchery(HatcheryType.MainMacro),
                    Make.Extractor,
                    Make.Zergling // scout
                })
                //.Concat(While(() => UsedSupply < 15, () => Make.Drone))
                .Concat(Repeat(() => Make.Drone, 4))
                .Concat(Make.Overlord.Yield())
                .Concat(Repeat(() => Make.Drone, 2))
                //.Concat(While(() => Game.Self.Gas < 100, () => Make.Drone))
                .Concat(Make.Lair.Yield())
                .Concat(Repeat(() => Make.Drone, 3))
                //.Concat(While(() => Game.Self.Gas < 100 || CompletionPercentage(UnitType.Zerg_Lair) < 10, () => Make.Drone))
                .Concat(Make.ZerglingSpeed.Yield())
                .Concat(Make.Extractor.Yield())
                .Concat(Repeat(() => Make.Drone, 4))
                .Concat(Make.Overlord.Yield())
                .Concat(Repeat(() => Make.Drone, 4))
                .Concat(Make.Spire.Yield())
                .Concat(Repeat(() => Make.Zergling, 6))
                .Concat(Repeat(() => Make.Overlord, 3))
                .Concat(Repeat(() => Make.Mutalisk, 9))
                .Concat(Do.SendDroneToThird.Yield())
                .Concat(Make.HydraliskDen.Yield())
                .Concat(Make.QueensNest.Yield())
                .Concat(Repeat(() => Make.Drone, 4))
                .Concat(Make.Hatchery(HatcheryType.ThirdExp).Yield())
                .Concat(Make.LurkerAspect.Yield())
                .Concat(Make.Hive.Yield())
                .Concat(Make.DefilerMound.Yield())
                .Concat(Make.UltraliskCavern.Yield())
                .Concat(Make.NydusCanal.Yield())
                .Concat(Make.NydusExit.Yield())
                .Concat(While(() => true, () => Make.Zergling));

            //.Concat(While(() => !HaveBuilding(UnitType.Zerg_Lair) && CompletionPercentage(UnitType.Zerg_Lair) < 90, () => Make.Drone));
        }

        public IEnumerable<Step> Repeat(Func<Step> step, int count) => Enumerable.Range(0, count).Select(_ => step());

        public IEnumerable<Step> While(Func<bool> predicate, Func<Step> step)
        {
            while (predicate()) yield return step();
        }

        public int UsedSupply => Game.Self.SupplyUsed / 2;

        public bool HaveBuilding(UnitType ut)
        {
            var buildingOfType = Game.Self.Units.Where(u => u.UnitType.IsBuilding).Where(b => b.IsCompleted).ToList();
            return buildingOfType.Any(b => b.Is(ut));
        }

        public int CompletionPercentage(UnitType ut)
        {
            var unitsOfType = Game.Self.Units.Where(u => u.Is(ut)).ToList();
            if (!unitsOfType.Any()) return 0;
            var target = unitsOfType.First();
            if (target.IsCompleted) return 100;
            var buildTime = UnitTypes.All[ut].Price.TimeFrames;
            var builtCurently = buildTime - target.RemainingBuildTime;
            var percentage = (int)Math.Round((double)builtCurently / buildTime * 100);
            return percentage;
        }
    }

    //12 - Hatchery @ expansion
    //11 - Spawning Pool
    //13 - Hatchery
    //12 - Extractor
    //@100% Spawning Pool - 2 Zerglings
    //16 - Overlord
    //@100 Gas:
    //Lair
    //Zergling Speed
    //Extractor at expansion
    //@100% Lair - Spire
    //24 - Overlord
    //25..27 - Drones
    //28..33 Zerglings
    //33 - Expansion or Sunken Colonies to adapt to the Terran builds
    //@50% Spire - 3 Overlords
    //Save Larvae
    //33 - 9 Mutalisks
    //@50 Gas - Hydralisk Den
    //@100-200 Gas:
    //2 more Mutalisks
    //1 Overlord
    //@75% Hydralisk Den - Evolution Chamber
    //@200 Gas - Lurker Aspect
    //@150 Gas - Carapace Upgrade
}