namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class ThreeWorkersOnGas : BaseBehavior
    {
        public ThreeWorkersOnGas(TilePosition basePosition) : base(basePosition)
        {
        }

        public override void Execute()
        {
            var workersGatheringGas = BaseWorkers.Where(w => w.IsGatheringGas).ToList();
            var hasNotEnough = workersGatheringGas.Count < 3 && HasBuilding(UnitType.Zerg_Extractor);
            if (hasNotEnough && BaseWorkers.Any())
            {
                GatherClosestGas(GetWorker());
                return;
            }

            var hasTooMany = workersGatheringGas.Count > 3;
            if (hasTooMany) GatherClosestMineral(workersGatheringGas.First());
        }
    }
}