namespace SmakenziBot.BuildOrder.Prerequisities
{
    using System.Linq;
    using BroodWar.Api;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class UnitExistsPrerequisite : Prerequisite
    {
        private readonly UnitType _unitType;

        public UnitExistsPrerequisite(UnitType unitType)
        {
            _unitType = unitType;
        }

        public override bool IsMet() => Game.Self.Units.Any(x => x.UnitType.Type == _unitType);

    }
}