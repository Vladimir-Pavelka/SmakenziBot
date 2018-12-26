namespace SmakenziBot.BuildOrder.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api.Enum;
    using Prerequisities;
    using Utils;

    public class MorphUpgradeBuildingStep : Step<UnitType>
    {
        public MorphUpgradeBuildingStep(UnitType target, params Prerequisite[] extraPrerequisities) : this(target, extraPrerequisities.ToList())
        {
        }

        public MorphUpgradeBuildingStep(UnitType target, IEnumerable<Prerequisite> extraPrerequisities)
        {
            var unitType = UnitTypes.All[target];

            var defaultPrerequisites = new Prerequisite[]
            {
                new ResourcePrerequisite(unitType.Price.Minerals, unitType.Price.Gas)
            };

            var unitPrerequisites = unitType.RequiredUnits.Keys.Where(u => !u.IsBuilding).Select(x => new UnitExistsPrerequisite(x.Type));
            var buildingPrerequisites = unitType.RequiredUnits.Keys.Where(u => u.IsBuilding).Select(x => new BuildingExistsPrerequisite(x.Type));

            Prerequisites = defaultPrerequisites.Concat(unitPrerequisites).Concat(buildingPrerequisites).Concat(extraPrerequisities).ToList();
            Target = target;
            WhatMorphs = unitType.WhatBuilds.Item1.Type;
        }

        public UnitType WhatMorphs { get; }
    }
}