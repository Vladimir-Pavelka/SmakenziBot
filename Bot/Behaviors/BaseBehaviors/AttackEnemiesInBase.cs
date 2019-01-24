namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using NBWTA.Result;
    using NBWTA.Utils;

    public class AttackEnemiesInBase : BaseBehavior
    {
        public AttackEnemiesInBase(MapRegion basePosition) : base(basePosition)
        {
        }

        public override void Execute()
        {
            if (!EnemiesInBase.Any()) return;
            OwnFighters.Where(u => u.IsIdle)
                .ForEach(u =>
                {
                    MyUnits.SetActivity(u, nameof(AttackEnemiesInBase));
                    u.Attack(EnemiesInBase.First().Position, false);
                });
        }
    }
}