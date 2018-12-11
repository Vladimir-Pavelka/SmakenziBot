namespace SmakenziBot.BuildOrder.Prerequisities
{
    using System.Linq;
    using BroodWar.Api;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class BuildingPrerequisite : Prerequisite
    {
        private readonly UnitType _buildingPrerequisite;

        public BuildingPrerequisite(UnitType buildingType)
        {
            _buildingPrerequisite = buildingType;
        }

        public override bool IsMet() => Game.Self.Units.Where(x => x.UnitType.IsBuilding).Where(x => x.IsCompleted).Any(x => x.UnitType.Type == _buildingPrerequisite);
    }
}