namespace SmakenziBot.BuildOrder.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using Prerequisities;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class ConstructBuildingStep : Step<UnitType>
    {
        public ConstructBuildingStep(UnitType target, TilePosition buildLocation = null, params Prerequisite[] prerequisites) : this(target, buildLocation, prerequisites.ToList())
        {
        }

        public ConstructBuildingStep(UnitType target, TilePosition buildLocation, IEnumerable<Prerequisite> extraPrerequisites)
        {
            BuildLocation = buildLocation;

            var unitType = UnitTypes.All[target];
            var defaultPrerequisites = new Prerequisite[]
            {
                new ResourcePrerequisite(unitType.Price.Minerals, unitType.Price.Gas)
            };
            var unitPrerequisites = unitType.RequiredUnits.Keys.Where(u => !u.IsBuilding).Select(x => new UnitExistsPrerequisite(x.Type));
            var buildingPrerequisites = unitType.RequiredUnits.Keys.Where(u => u.IsBuilding).Select(x => new BuildingExistsPrerequisite(x.Type));

            Prerequisites = defaultPrerequisites.Concat(unitPrerequisites)
                .Concat(buildingPrerequisites).Concat(extraPrerequisites)
                .ToList();

            Target = target;
        }

        public TilePosition BuildLocation { get; }
    }
}