namespace SmakenziBot.BuildOrder.Steps
{
    using System.Linq;
    using BroodWar.Api.Enum;
    using Prerequisities;

    public class CancelConstructBuildingStep : Step<UnitType>
    {
        public CancelConstructBuildingStep(UnitType target)
        {
            Prerequisites = Enumerable.Empty<Prerequisite>();
            Target = target;
        }

        public override string ToString() => $"Cancel {Target}";
    }
}