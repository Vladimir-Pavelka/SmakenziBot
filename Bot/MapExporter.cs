namespace SmakenziBot
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;

    public static class MapExporter
    {
        private static readonly Color UnwalkableColor = Color.Gray;
        private static readonly Color MineralColor = Color.DodgerBlue;
        private static readonly Color GeyserColor = Color.Green;

        private static readonly Dictionary<Resolution, int> MapPixelBuildTileSize = new Dictionary<Resolution, int> { { Resolution.Pixel, 32 }, { Resolution.WalkTiles, 4 } };
        private static readonly Dictionary<Resolution, int> MapPixelWalkTileSize = new Dictionary<Resolution, int> { { Resolution.Pixel, 8 }, { Resolution.WalkTiles, 1 } };

        public static void ExportMap()
        {
            const string validFileNameChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";
            var mapName = string.Join("", Game.MapName.Where(validFileNameChars.Contains));
            var folder = $"../../../ExportedMaps/{mapName}";
            Directory.CreateDirectory(folder);
            ExportMap($"{folder}/{mapName}.bmp");
            ExportMap($"{folder}/{mapName}_walls.bmp", false);
        }

        /// <summary>
        /// * Position - pixels, the highest resolution
        /// * Walk Tile - 8x8 square of pixels. Walkability data is available at this resolution.
        /// * Build Tile - 4x4 square of walk tiles, 32x32 square of pixels. Buildability data is available at this resolution, tiles seen in game.
        ///   For example, a Command Center occupies an area of 4x3 build tiles.
        /// </summary>
        public static void ExportMap(string fileName, bool showResources = true, Resolution mapResolution = Resolution.WalkTiles)
        {
            var buildTileSize = MapPixelBuildTileSize[mapResolution];
            var map = new Bitmap(Game.MapWidth * buildTileSize, Game.MapHeight * buildTileSize);
            SetWalkabilityInfoToMap(map, mapResolution);
            if (showResources) SetResourcesInfoToMap(map, mapResolution);
            map.Save(fileName);
        }

        private static void SetWalkabilityInfoToMap(Bitmap map, Resolution mapResolution)
        {
            var walkTileSize = MapPixelWalkTileSize[mapResolution];

            var xWalkTileRange = Enumerable.Range(0, map.Width / walkTileSize);
            var yWalkTileRange = Enumerable.Range(0, map.Height / walkTileSize);
            xWalkTileRange.SelectMany(x => yWalkTileRange.Select(y => new Point(x, y)))
                .Where(walkTile => !Game.IsWalkable(walkTile.X, walkTile.Y))
                .Select(walktile => new Point(walktile.X * walkTileSize, walktile.Y * walkTileSize))
                .Select(mapPixel => YieldPointRange(mapPixel, walkTileSize, walkTileSize))
                .ForEach(mapPixels => mapPixels.ForEach(mapPixel => map.SetPixel(mapPixel.X, mapPixel.Y, UnwalkableColor)));
        }

        private static void SetResourcesInfoToMap(Bitmap map, Resolution mapResolution)
        {
            var buildTileSize = MapPixelBuildTileSize[mapResolution];

            Game.StaticMinerals.Select(m => m.TilePosition).Select(p => new Point(p.X * buildTileSize, p.Y * buildTileSize))
                .Select(p => YieldPointRange(p, buildTileSize * 2, buildTileSize))
                .ForEach(pixels => pixels.ForEach(p => map.SetPixel(p.X, p.Y, MineralColor)));

            Game.StaticGeysers.Select(g => g.TilePosition).Select(p => new Point(p.X * buildTileSize, p.Y * buildTileSize))
                .Select(p => YieldPointRange(p, buildTileSize * 4, buildTileSize * 2))
                .ForEach(pixels => pixels.ForEach(p => map.SetPixel(p.X, p.Y, GeyserColor)));
        }

        private static IEnumerable<Point> YieldPointRange(Point point, int xCount, int yCount) =>
            Enumerable.Range(point.X, xCount).SelectMany(x =>
                Enumerable.Range(point.Y, yCount).Select(y =>
                    new Point(x, y)));

        private class Point
        {
            public int X { get; }
            public int Y { get; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        public enum Resolution
        {
            Pixel,
            WalkTiles
        }
    }
}