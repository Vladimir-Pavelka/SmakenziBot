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
            if (!HasCompletedBuilding(UnitType.Zerg_Extractor)) return;

            var workersGatheringMinerals = MineralWorkers.ToList();
            var workersGatheringGas = GasWorkers.ToList();

            var hasNotEnoughOnGas = workersGatheringGas.Count < 3;
            var isMineralLineHealthy = workersGatheringMinerals.Count > 6;
            if (hasNotEnoughOnGas && isMineralLineHealthy && workersGatheringMinerals.Any())
            {
                GatherClosestGas(workersGatheringMinerals.First());
                return;
            }

            var hasTooManyOnGas = workersGatheringGas.Count > 3;
            var isMineralLineStarved = workersGatheringMinerals.Count < 6;
            if (hasTooManyOnGas || isMineralLineStarved && workersGatheringGas.Any())
                GatherClosestMineral(workersGatheringGas.First());
        }
    }
}