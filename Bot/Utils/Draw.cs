namespace SmakenziBot.Utils
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

        public static void Chokes(IEnumerable<ChokeRegion> chokeRegions) =>
            Chokes(chokeRegions.SelectMany(ch => ch.ContentTiles));

        public static void Chokes(IEnumerable<(int x, int y)> chokePoints) =>
             chokePoints.ForEach(t => Game.DrawDot(new Position(t.x * 8 + 4, t.y * 8 + 4), Color.Red));

        public static void ChokeLine(IReadOnlyCollection<(int x, int y)> chokeLinePoints)
        {
            var firstPoint = chokeLinePoints.First();
            var lastPoint = chokeLinePoints.Last();
            var lineStart = new Position(firstPoint.x * 8 + 4, firstPoint.y * 8 + 4);
            var lineEnd = new Position(lastPoint.x * 8 + 4, lastPoint.y * 8 + 4);

            Game.DrawLine(lineStart, lineEnd, Color.Red);
        }

        public static void ResourceClusters(IEnumerable<IEnumerable<(int x, int y)>> clusters) =>
            clusters.Select((c, idx) => (cluster: c, idx: idx))
                .ForEach(cidx => ResourceCluster(cidx.cluster, cidx.idx));

        private static void ResourceCluster(IEnumerable<(int x, int y)> cluster, int idx)
        {
            var positions = cluster.Select(tile => new Position(tile.x * 32 + 16, tile.y * 32 + 16));
            positions.ForEach(p => Game.DrawText(p, $"{idx}"));
        }

        public static void MainBuildingPlacements(IEnumerable<(int x, int y)> mainBuildingLocations)
        {
            mainBuildingLocations.ForEach(location =>
            {
                var topLeft = new Position(location.x * 32, location.y * 32);
                var bottomRight = new Position((location.x + 4) * 32, (location.y + 3) * 32);
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

        public static void Region(MapRegion region)
        {
            var avgX = (int)Math.Round(region.ContentTiles.Average(t => t.x));
            var avgY = (int)Math.Round(region.ContentTiles.Average(t => t.y));
            var regionCenter = new WalkPosition(avgX, avgY).ToPixelTile();
            Game.DrawCircle(regionCenter, 50, Color.Black, false);
        }
    }
}