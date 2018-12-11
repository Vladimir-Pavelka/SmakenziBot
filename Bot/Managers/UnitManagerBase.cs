namespace SmakenziBot.Managers
{
    using System.Collections.Generic;
    using BroodWar.Api;

    public class UnitManagerBase<TStatus>
    {
        protected readonly IDictionary<Unit, TStatus> AssignedUnits = new Dictionary<Unit, TStatus>();

        public bool Controls(Unit unit) => AssignedUnits.ContainsKey(unit);

        public void Assign(Unit unit) => AssignedUnits[unit] = default(TStatus);
        public void Release(Unit unit) => AssignedUnits.Remove(unit);
    }
}