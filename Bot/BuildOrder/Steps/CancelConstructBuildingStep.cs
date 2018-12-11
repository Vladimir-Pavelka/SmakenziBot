namespace SmakenziBot.BuildOrder.Steps
{
    using System.Linq;
    using BroodWar.Api.Enum;
    using Prerequisities;

    public class CancelConstructBuildingStep : Step<UnitType>
    {
        public CancelConstructBuildingStep(UnitType item)
        {
            Prerequisites = Enumerable.Empty<Prerequisite>();
            Item = item;
        }

        public override string ToString() => $"Cancel {Item}";
    }
}