namespace SmakenziBot.BuildOrder.Steps
{
    using BroodWar.Api.Enum;

    public class HatcheryBuildingStep : ConstructBuildingStep
    {
        public HatcheryType HatcheryType { get; }

        public HatcheryBuildingStep(HatcheryType type) : base(UnitType.Zerg_Hatchery)
        {
            HatcheryType = type;
        }
    }

    public enum HatcheryType
    {
        MainMacro,
        NaturalExp,
        ThirdExp,
    }
}