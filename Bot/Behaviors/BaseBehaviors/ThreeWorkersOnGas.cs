namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using NBWTA.Result;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class ThreeWorkersOnGas : BaseBehavior
    {
        public ThreeWorkersOnGas(MapRegion basePosition) : base(basePosition)
        {
        }

        public override void Execute()
        {
            if (!HasBuilding(UnitType.Zerg_Extractor)) return;

            var workersGatheringGas = OwnWorkers.Where(w => w.IsGatheringGas).ToList();
            var workersGatheringMinerals = OwnWorkers.Where(w => w.IsGatheringMinerals).ToList();
            var hasNotEnough = workersGatheringGas.Count < 3;
            if (hasNotEnough && workersGatheringMinerals.Count > 6 && OwnWorkers.Any())
            {
                GatherClosestGas(GetWorker());
                return;
            }

            var hasTooMany = workersGatheringGas.Count > 3;
            if (hasTooMany || workersGatheringMinerals.Count < 6 && workersGatheringGas.Any())
                GatherClosestMineral(workersGatheringGas.First());
        }
    }
}