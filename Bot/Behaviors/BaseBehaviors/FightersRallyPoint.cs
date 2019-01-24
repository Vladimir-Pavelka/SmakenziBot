namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Result;
    using NBWTA.Utils;
    using Utils;

    public class FightersRallyPoint : BaseBehavior
    {
        private readonly Position _rellyFightersTo;

        public FightersRallyPoint(MapRegion basePosition, MapRegion rellyFightersTo) : base(basePosition)
        {
            _rellyFightersTo = rellyFightersTo.ContentTiles.AveragePosition().AsWalkTile().ToPixelTile();
        }

        public override void Execute()
        {
            OwnFighters.Where(u => u.IsIdle).ForEach(u =>
            {
                u.Move(_rellyFightersTo, false);
                MyUnits.SetActivity(u, nameof(FightersRallyPoint));
            });
        }
    }
}