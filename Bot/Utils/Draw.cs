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

        public static void Chokes(IEnumerable<ChokeRegion> chokeRegions)
        {
            chokeRegions.SelectMany(ch => ch.ContentTiles)
                .ForEach(t => Game.DrawDot(new Position(t.x * 8 + 4, t.y * 8 + 4), Color.Red));
        }

        public static void ResourceClusters(IEnumerable<ResourceSite> sites)
        {
            sites.Select((s, idx) => (resources: s.MineralsBuildTiles.Concat(s.GeysersBuildTiles), idx: idx))
                .ForEach(sidx => sidx.resources.ForEach(r => Game.DrawText(new Position(r.x * 32, r.y * 32), $"{sidx.idx}")));
        }

        public static void MainBuildingPlacements(IReadOnlyCollection<ResourceSite> sites)
        {
            sites.ForEach(site =>
            {
                var location = site.OptimalResourceDepotBuildTile;
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
    }
}