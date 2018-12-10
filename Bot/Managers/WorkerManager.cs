namespace SmakenziBot.Managers
{
    using System.Linq;
    using BroodWar.Api;

    public class WorkerManager : UnitManagerBase<WorkerStatus>
    {
        public void OnGameStart()
        {
            AssignedUnits.Keys.ToList().ForEach(GatherClosestMineral);
        }

        public void OnFrame()
        {
            AssignedUnits.Where(kvp => kvp.Value == WorkerStatus.Idle).Select(kvp => kvp.Key).ToList().ForEach(GatherClosestMineral);
        }

        private void GatherClosestMineral(Unit worker)
        {
            var closestMineral = Game.Minerals.OrderBy(worker.Distance).First(x => !x.IsBeingGathered);
            worker.Gather(closestMineral, false);
            AssignedUnits[worker] = WorkerStatus.MiningMineral;
        }
    }
}