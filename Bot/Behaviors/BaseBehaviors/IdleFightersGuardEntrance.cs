namespace SmakenziBot.Behaviors.BaseBehaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Result;
    using NBWTA.Utils;
    using Utils;

    public class IdleFightersGuardEntrance : BaseBehavior
    {
        private const int ArcRadiusPx = 6 * 32;
        private readonly ChokeRegion _entrance;
        private readonly (int x, int y) _entranceCenter;
        private readonly IReadOnlyList<(int x, int y)> _entranceArc;
        private readonly Random _rnd = new Random();

        public IdleFightersGuardEntrance(MapRegion basePosition, ChokeRegion entrance) : base(basePosition)
        {
            _entrance = entrance;
            _entranceCenter = _entrance.ContentTiles.AveragePosition();
            const int arcRadiusWalkTiles = ArcRadiusPx / 8;
            var gridCircle = GridCircle.YieldIndices(_entranceCenter, arcRadiusWalkTiles);
            _entranceArc = gridCircle
                .Where(basePosition.ContentTiles.Contains)
                .ToList();
        }

        public override void Execute()
        {
            if (_entrance == null || !_entranceArc.Any()) return;

            var unitsToMove = OwnFighters.Where(u => u.IsIdle).Where(u => !IsNearEntrance(u));
            unitsToMove.ForEach(u =>
            {
                MyUnits.SetActivity(u, nameof(IdleFightersGuardEntrance));
                u.Move(RandomEntrenceArcPosition, false);
            });
        }

        private bool IsNearEntrance(Unit u) =>
            u.Position.CalcApproximateDistance(_entranceCenter.AsWalkTile().ToPixelTile()) < ArcRadiusPx + 64;

        private Position RandomEntrenceArcPosition =>
            _entranceArc[_rnd.Next(0, _entranceArc.Count - 1)].AsWalkTile().ToPixelTile();
    }
}