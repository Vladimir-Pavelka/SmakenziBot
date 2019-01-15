namespace SmakenziBot
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public static class MyUnits
    {
        private static readonly IDictionary<Unit, string> _trackedUnits = new Dictionary<Unit, string>();

        public static void SetActivity(Unit u, string activityName)
        {
            RemoveDeadUnits();
            _trackedUnits[u] = activityName;
        }

        private static void RemoveDeadUnits() =>
            _trackedUnits.Where(kvp => !kvp.Key.Exists || kvp.Key.UnitType.IsBuilding).ToList()
            .ForEach(kvp => _trackedUnits.Remove(kvp));

        public static IDictionary<Unit, string> TrackedUnits => _trackedUnits;
    }

    //public static void Attack(Unit unit, Unit target)
    //{
    //    if (!unit.Exists)
    //    {
    //        _trackedUnits.Remove(unit);
    //        return;
    //    }

    //    if (target.)

    //    _trackedUnits[unit] = ;

    //}

    //public static void Attack(Unit unit, Position target)
    //{
    //    _trackedUnits[unit] = activity;

    //    //}

    //    public static void Move(Unit unit, Position destination)
    //    {

    //    }

    //    public static void Construct(Unit builder, UnitType building, TilePosition buildSite)
    //    {

    //    }

    //    public static IReadOnlyCollection<Unit> TrackedUnits =>
    //}

    //public enum UnitActivity
    //{
    //    Idle,
    //    OnAttackMove,
    //    OnConstructMove,
    //    OnMove,
    //    Attacking,
    //    Holding
    //}
}