namespace SmakenziBot.BuildOrder.Steps
{
    using System;
    using BroodWar.Api.Enum;

    public class HatcheryBuildingStep : ConstructBuildingStep
    {
        public HatcheryType HatcheryType { get; }

        public HatcheryBuildingStep(HatcheryType type) : base(UnitType.Zerg_Hatchery)
        {
            HatcheryType = type;
        }
    }

    [Flags]
    public enum HatcheryType
    {
        MainMacro = 1,
        NaturalExp = 2,
        ThirdExp = 4,
        Expansion = NaturalExp | ThirdExp
    }
}