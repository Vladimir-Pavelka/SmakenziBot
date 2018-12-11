namespace SmakenziBot.BuildOrder.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api.Enum;
    using Prerequisities;

    public class MorphUnitStep : Step<UnitType>
    {
        private static readonly IDictionary<UnitType, int> _requiredSupply = new Dictionary<UnitType, int>
        {
            { UnitType.Zerg_Overlord, 0},
            { UnitType.Zerg_Drone, 1},
            { UnitType.Zerg_Zergling, 1},
            { UnitType.Zerg_Hydralisk, 1}
        };

        public MorphUnitStep(UnitType item, params Prerequisite[] prerequisites) : this(item, prerequisites.ToList())
        {
        }

        public MorphUnitStep(UnitType item, IEnumerable<Prerequisite> prerequisites)
        {
            var defaultPrerequisites = new Prerequisite[]
                {new UnitExistsPrerequisite(UnitType.Zerg_Larva), new AvailableSupplyPrerequisite(_requiredSupply[item])};

            Prerequisites = defaultPrerequisites.Concat(prerequisites).ToList();
            Item = item;
        }
    }
}