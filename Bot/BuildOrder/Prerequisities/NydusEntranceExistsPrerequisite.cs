namespace SmakenziBot.BuildOrder.Prerequisities
{
    using System.Linq;
    using BroodWar.Api;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class NydusEntranceExistsPrerequisite : Prerequisite
    {
        public override bool IsMet() => Game.Self.Units.Where(u => u.Is(UnitType.Zerg_Nydus_Canal)).Any(n => n.NydusExit == null);
    }
}