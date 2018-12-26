namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;

    public class WorkersAttackClosestEnemy : BaseBehavior
    {
        public WorkersAttackClosestEnemy(TilePosition basePosition) : base(basePosition)
        {
        }

        public override void Execute()
        {
            BaseWorkers.Where(u => u.IsUnderAttack).ForEach(worker =>
            {
                var closeUnits = worker.UnitsInRadius(64);
                var closeEnemyUnits = closeUnits.Where(Game.Enemy.Units.Contains).ToList();
                if (!closeEnemyUnits.Any()) return;
                var closeAlliedUnits = closeUnits.Where(Game.Self.Units.Contains)
                    .Where(w => w.IsGatheringMinerals || w.IsGatheringGas).Where(w => !ShouldFlee(w));
                closeAlliedUnits.ForEach(u => u.Attack(closeEnemyUnits.First(), false));

                if (ShouldFlee(worker)) GatherBaseMineralFarFromAttacker(worker, closeEnemyUnits.First());
            });
        }

        private static bool ShouldFlee(Unit worker) => (double)worker.HitPoints / worker.InitialHitPoints < 0.66;

        private void GatherBaseMineralFarFromAttacker(Unit worker, Unit attacker)
        {
            if (!BaseMinerals.Any()) return;
            var farMineral = BaseMinerals.OrderByDescending(m => m.TilePosition.CalcApproximateDistance(attacker.TilePosition)).First();
            worker.Gather(farMineral, false);
        }
    }
}