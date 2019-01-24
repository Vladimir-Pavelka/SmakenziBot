namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using NBWTA.Result;
    using NBWTA.Utils;
    using Utils;

    public class CounterAttackStackWorkers : BaseBehavior
    {
        public CounterAttackStackWorkers(MapRegion basePosition) : base(basePosition)
        {
        }

        public override void Execute()
        {
            if (!EnemiesInBase.Any() || !AreEnemiesInMineralLine()) return;
            if (!AreWorkersStacked() && !AreWorkersAttacking())
            {
                OrderToStackOnMineral();
                return;
            }

            AttackClosestEnemy();
        }

        private bool AreEnemiesInMineralLine() =>
           EnemiesNearWorkers.Any() && MineralWorkers.Any();

        private bool AreWorkersStacked()
        {
            var averagePosition = MineralWorkers.ToList().AveragePosition();
            return MineralWorkers.All(w => w.TilePosition.CalcApproximateDistance(averagePosition) <= 1);
        }

        private bool AreWorkersAttacking() => MineralWorkers.Any(w => w.IsAttacking);

        private void OrderToStackOnMineral()
        {
            var averagePosition = MineralWorkers.ToList().AveragePosition();
            var closestMineral = BaseMinerals.ClosestTo(averagePosition);
            MineralWorkers.ForEach(w => w.Gather(closestMineral, false));
        }

        private void AttackClosestEnemy()
        {
            var averagePosition = MineralWorkers.ToList().AveragePosition();
            var closestEnemy = EnemiesNearWorkers.ClosestTo(averagePosition);
            MineralWorkers.ForEach(w => w.Attack(closestEnemy, false));
        }
    }
}