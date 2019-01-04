namespace SmakenziBot
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;

    public class Squad
    {
        private IReadOnlyCollection<Unit> Units { get; }

        public Squad(IReadOnlyCollection<Unit> units)
        {
            Units = units;
        }

        private IEnumerable<Unit> EnemyUnitsInSight =>
            Units.SelectMany(u =>
            {
                var unitsSightRange = Game.Self.SightRange(u.UnitType.Type);
                return u.UnitsInRadius(unitsSightRange).Where(Game.Enemy.Units.Contains);
            })
            .Distinct();
    }
}