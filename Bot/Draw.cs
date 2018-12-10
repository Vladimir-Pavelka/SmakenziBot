namespace SmakenziBot
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Result;
    using NBWTA.Utils;

    public static class Draw
    {
        public static void Regions(IReadOnlyCollection<MapRegion> mapRegions)
        {
        }

        public static void Chokes(IEnumerable<ChokeRegion> chokeRegions)
        {
            chokeRegions.SelectMany(ch => ch.ContentTiles)
                .ForEach(t => Game.DrawDot(new Position(t.x * 8 + 4, t.y * 8 + 4), Color.Red));
        }

        public static void ResourceClusters(IEnumerable<HashSet<Unit>> clusters)
        {
            clusters.Select((c, idx) => (cluster: c, idx: idx))
                .ForEach(cidx => cidx.cluster.ForEach(x => Game.DrawText(x.Position, $"{cidx.idx}")));
        }

        public static void MainBuildingPlacements(IReadOnlyCollection<TilePosition> mainBuildingLocations)
        {
            mainBuildingLocations.ForEach(location =>
            {
                var topLeft = new Position(location.X * 32, location.Y * 32);
                var bottomRight = new Position((location.X + 4) * 32, (location.Y + 3) * 32);
                Game.DrawBox(topLeft, bottomRight, Color.CornflowerBlue, false);
            });
        }

        private static Color GetRandomColor(int? seed = null)
        {
            var random = new Random(seed ?? DateTime.Now.Millisecond);
            var r = random.Next(0, 255);
            var g = random.Next(0, 255);
            var b = random.Next(0, 255);

            return Color.FromArgb(r, g, b);
        }
    }
}