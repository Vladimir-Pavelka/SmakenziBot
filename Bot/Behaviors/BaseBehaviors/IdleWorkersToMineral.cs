namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using NBWTA.Result;
    using NBWTA.Utils;

    public class IdleWorkersToMineral : BaseBehavior
    {
        public IdleWorkersToMineral(MapRegion basePosition) : base(basePosition)
        {
        }

        public override void Execute() => BaseWorkers.Where(w => w.IsIdle).ForEach(w =>
        {
            MyUnits.SetActivity(w, nameof(IdleWorkersToMineral));
            GatherClosestMineral(w);
        });
    }
}