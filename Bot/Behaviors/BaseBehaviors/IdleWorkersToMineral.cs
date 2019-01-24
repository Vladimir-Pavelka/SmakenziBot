namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using BuildOrder.Steps;
    using NBWTA.Result;
    using NBWTA.Utils;

    public class IdleWorkersToMineral : BaseBehavior
    {
        public IdleWorkersToMineral(MapRegion basePosition) : base(basePosition)
        {
        }

        public override void Execute() => OwnWorkers.Where(w => w.IsIdle)
            .Where(w => !MyUnits.TrackedUnits.ContainsKey(w) || !MyUnits.TrackedUnits[w].StartsWith("MoveDroneTo"))
            .ForEach(w =>
            {
                MyUnits.SetActivity(w, nameof(IdleWorkersToMineral));
                GatherClosestMineral(w);
            });
    }
}