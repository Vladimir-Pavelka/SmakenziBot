namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Result;
    using NBWTA.Utils;
    using Utils;

    public class WorkersAttackClosestEnemy : BaseBehavior
    {
        public WorkersAttackClosestEnemy(MapRegion basePosition) : base(basePosition)
        {
        }

        public override void Execute()
        {
            OwnWorkers.Where(u => u.IsUnderAttack).ForEach(worker =>
            {
                var closeUnits = worker.UnitsInRadius(64);
                var closeEnemyUnits = closeUnits.Where(Game.Enemy.Units.Contains).ToList();
                if (!closeEnemyUnits.Any()) return;
                var closeAlliedUnits = closeUnits.Where(Game.Self.Units.Contains)
                    .Where(w => w.IsGatheringMinerals || w.IsGatheringGas).Where(w => !ShouldFlee(w));
                closeAlliedUnits.ForEach(u =>
                {
                    MyUnits.SetActivity(u, nameof(WorkersAttackClosestEnemy));
                    u.Attack(closeEnemyUnits.First(), false);
                });

                if (ShouldFlee(worker)) GatherBaseMineralFarFromAttacker(worker, closeEnemyUnits.First());
            });
        }

        private static bool ShouldFlee(Unit worker) => (double)worker.HitPoints / worker.InitialHitPoints < 0.66;

        private void GatherBaseMineralFarFromAttacker(Unit worker, Unit attacker)
        {
            if (!BaseMinerals.Any()) return;
            var farMineral = BaseMinerals.MaxBy(m => m.TilePosition.CalcApproximateDistance(attacker.TilePosition));
            worker.Gather(farMineral, false);
        }
    }
}