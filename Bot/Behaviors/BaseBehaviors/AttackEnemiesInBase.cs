namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;

    public class AttackEnemiesInBase : BaseBehavior
    {
        public AttackEnemiesInBase(TilePosition basePosition) : base(basePosition)
        {
        }

        public override void Execute()
        {
            if (!EnemiesInBase.Any()) return;
            BaseCombatUnits.Where(u => u.IsIdle)
                .ForEach(u => u.Attack(EnemiesInBase.First().Position, false));
        }
    }
}