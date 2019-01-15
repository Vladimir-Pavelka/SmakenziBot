namespace SmakenziBot.Behaviors.CombatBehaviors
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;
    using Utils;

    public class TowersAttackLowestHp : CombatBehavior
    {
        public override void Execute()
        {
            if (Game.FrameCount % 11 != 0) return;
            var combatBuildings = Game.Self.Units.Where(u => u.UnitType.IsBuilding).Where(x => x.UnitType.CanAttack);

            combatBuildings.Select(cb => (cb: cb, enemies: EnemiesInWeaponRange(cb)))
                .Where(x => x.enemies.Any())
                .ForEach(x => x.cb.Attack(x.enemies.MinBy(e => e.HitPoints + e.Shields), false));
        }

        private static HashSet<Unit> EnemiesInWeaponRange(Unit u) =>
            u.Weapons()
            .SelectMany(w => u.UnitsInRadius(w.MaxRange))
            .Where(Game.Enemy.Units.Contains)
            .ToHashSet();
    }
}