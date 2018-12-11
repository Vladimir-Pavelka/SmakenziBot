namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;
    using Utils;

    public class CounterAttackStackWorkers : BaseBehavior
    {
        public CounterAttackStackWorkers(TilePosition basePosition) : base(basePosition)
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
           EnemiesNearMineralLine.Any() && WorkersNearMineralLine.Any();

        private bool AreWorkersStacked()
        {
            var averagePosition = WorkersNearMineralLine.ToList().AveragePosition();
            return WorkersNearMineralLine.All(w => w.TilePosition.CalcApproximateDistance(averagePosition) <= 1);
        }

        private bool AreWorkersAttacking() => WorkersNearMineralLine.Any(w => w.IsAttacking);

        private void OrderToStackOnMineral()
        {
            var averagePosition = WorkersNearMineralLine.ToList().AveragePosition();
            var closestMineral = BaseMinerals.ClosestTo(averagePosition);
            WorkersNearMineralLine.ForEach(w => w.Gather(closestMineral, false));
        }

        private void AttackClosestEnemy()
        {
            var averagePosition = WorkersNearMineralLine.ToList().AveragePosition();
            var closestEnemy = EnemiesNearMineralLine.ClosestTo(averagePosition);
            WorkersNearMineralLine.ForEach(w => w.Attack(closestEnemy, false));
        }
    }
}