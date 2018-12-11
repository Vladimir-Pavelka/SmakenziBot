namespace SmakenziBot.BuildOrder.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api.Enum;
    using Prerequisities;

    public class ConstructBuildingStep : Step<UnitType>
    {
        public ConstructBuildingStep(UnitType item, params Prerequisite[] prerequisites) : this(item, prerequisites.ToList())
        {
        }

        public ConstructBuildingStep(UnitType item, IEnumerable<Prerequisite> prerequisites)
        {
            var defaultPrerequisites = new Prerequisite[]
                {new UnitExistsPrerequisite(UnitType.Zerg_Drone)};

            Prerequisites = defaultPrerequisites.Concat(prerequisites).ToList();
            Item = item;
        }
    }
}