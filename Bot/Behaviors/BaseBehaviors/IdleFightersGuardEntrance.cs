namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Result;
    using NBWTA.Utils;
    using Utils;

    public class IdleFightersGuardEntrance : BaseBehavior
    {
        private readonly ChokeRegion _entrance;
        private readonly Position _entranceCenter;

        public IdleFightersGuardEntrance(TilePosition basePosition, ChokeRegion entrance) : base(basePosition)
        {
            _entrance = entrance;
            _entranceCenter = _entrance.ContentTiles.AveragePosition().AsWalkTile().ToPixelTile();
        }

        public override void Execute()
        {
            if (_entrance == null) return;
            BaseCombatUnits.Where(u => u.IsIdle).Where(u => !IsNearEntrance(u))
                .ForEach(u => u.Move(_entranceCenter, false));
        }

        private bool IsNearEntrance(Unit u) =>
            u.Position.CalcApproximateDistance(_entranceCenter) < 150;
    }
}