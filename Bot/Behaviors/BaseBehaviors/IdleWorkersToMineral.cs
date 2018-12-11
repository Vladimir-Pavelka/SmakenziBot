namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;

    public class IdleWorkersToMineral : BaseBehavior
    {
        public IdleWorkersToMineral(TilePosition basePosition) : base(basePosition)
        {
        }

        public override void Execute() => BaseWorkers.Where(w => w.IsIdle).ForEach(GatherClosestMineral);
    }
}