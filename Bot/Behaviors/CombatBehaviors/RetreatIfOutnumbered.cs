namespace SmakenziBot.Behaviors.CombatBehaviors
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;
    using Utils;

    public class RetreatIfOutnumbered : CombatBehavior
    {
        private readonly GameInfo _gameInfo;
        private readonly Position _mapCenter = (Game.MapWidth * 32 / 2, Game.MapHeight * 32 / 2).AsPixelTile();

        public RetreatIfOutnumbered(GameInfo gameInfo)
        {
            _gameInfo = gameInfo;
        }

        public override void Execute()
        {
            if (Game.FrameCount % 9 != 0) return;
            MyCombatUnits.Where(IsOutsideOfBase).Where(IsOutnumbered).ForEach(RetreatToClosestReinforcement);
        }

        private bool IsOutsideOfBase(Unit u) =>
            !_gameInfo.MyBases.Any(b => b.BaseRegion.ContentTiles.Contains(u.TilePosition.ToWalkTile().AsTuple()));

        private static bool IsOutnumbered(Unit u)
        {
            var unitsSightRange = Game.Self.SightRange(u.UnitType.Type) + 32;
            var unitsInSightRange = u.UnitsInRadius(unitsSightRange);
            var alliedFighters = unitsInSightRange.Where(Game.Self.Units.Contains).Where(x => x.IsFighter()).ToList();
            var enemyFighters = unitsInSightRange.Where(Game.Enemy.Units.Contains).Where(x => x.IsFighter()).ToList();
            return enemyFighters.Any() && AreOutnumbered(alliedFighters, enemyFighters);
        }

        private static bool AreOutnumbered(IReadOnlyCollection<Unit> allied, IReadOnlyCollection<Unit> enemy)
        {
            var myHealth = allied.Sum(u => u.HitPoints + u.Shields);
            var myDps = allied.Select(u => u.UnitType.GroundWeapon).Sum(w => DamagePerSecond(w));
            var enemyHealth = enemy.Sum(u => u.HitPoints + u.Shields);
            var enemyDps = enemy.Select(u => u.UnitType.GroundWeapon).Sum(w => DamagePerSecond(w));

            return myHealth / enemyDps < enemyHealth / myDps;
        }

        private static double DamagePerSecond(Weapon w) => 1.0 / w.DamageCooldown * w.DamageAmount;

        private void RetreatToClosestReinforcement(Unit u)
        {
            MyUnits.SetActivity(u, nameof(RetreatIfOutnumbered));
            var natural = _gameInfo.MyBases.FirstOrDefault(b => b.IsNatural);
            var any = _gameInfo.MyBases.FirstOrDefault();
            var retreatPoint = (natural ?? any)?.BaseRegion.ContentTiles.First().AsWalkTile().ToPixelTile() ?? _mapCenter;
            u.Move(retreatPoint, false);
        }
    }
}