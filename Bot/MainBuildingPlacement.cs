namespace SmakenziBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class MainBuildingPlacement
    {
        private const int MaxSearchAreaTiles = 10;
        private const int MinDistanceFromResourceTiles = 3;
        private const int MainBuildingWidthTiles = 4;
        private const int MainBuildingHeightTiles = 3;
        private const int MineralWidthTiles = 2;
        private const int MineralHeightTiles = 1;
        private const int GeyserWidthTiles = 4;
        private const int GeyserHeightTiles = 2;

        private readonly int _mapWidthTiles;
        private readonly int _mapHeightTiles;

        public MainBuildingPlacement(int mapWidthTiles, int mapHeightTiles)
        {
            _mapWidthTiles = mapWidthTiles;
            _mapHeightTiles = mapHeightTiles;
        }

        public IReadOnlyCollection<TilePosition> CalculatePlacements(IReadOnlyCollection<HashSet<Unit>> resourceSites, Func<TilePosition, bool> isBuildable)
        {
            return resourceSites.Select(site => CalculatePlacementForSingleSite(site, isBuildable)).ToList();
        }

        internal TilePosition CalculatePlacementForSingleSite(HashSet<Unit> resourceSite, Func<TilePosition, bool> isBuildable)
        {
            var allResourceXCoordinates = resourceSite.Select(resource => resource.TilePosition.X).ToList();
            var allResourceYCoordinates = resourceSite.Select(resource => resource.TilePosition.Y).ToList();

            var searchBoxMinX = GetMinXWithinMapBoundary(allResourceXCoordinates.Min() - MaxSearchAreaTiles);
            var searchBoxMinY = GetMinYWithinMapBoundary(allResourceYCoordinates.Min() - MaxSearchAreaTiles);
            var searchBoxMaxX = GetMaxXWithinMapBoundary(allResourceXCoordinates.Max() + MaxSearchAreaTiles);
            var searchBoxMaxY = GetMaxYWithinMapBoundary(allResourceYCoordinates.Max() + MaxSearchAreaTiles);

            var searchBoxTiles = YieldSearchBoxTiles(searchBoxMinX, searchBoxMaxX, searchBoxMinY, searchBoxMaxY);

            var mineralTopLefts = resourceSite.Where(x => x.UnitType.Type == UnitType.Resource_Mineral_Field).Select(x => x.TilePosition).ToList();
            var geyserTopLefts = resourceSite.Where(x => x.UnitType.Type == UnitType.Resource_Vespene_Geyser).Select(x => x.TilePosition).ToList();

            var mineralsOcupancy = mineralTopLefts.SelectMany(mineral => ToResourceOccupancy(mineral, MineralWidthTiles, MineralHeightTiles)).Distinct();
            var geysersOccupancy = geyserTopLefts.SelectMany(geyser => ToResourceOccupancy(geyser, GeyserWidthTiles, GeyserHeightTiles)).Distinct();

            var resourcesOccupancy = mineralsOcupancy.Concat(geysersOccupancy).Distinct();

            var buildableTiles = searchBoxTiles.Except(resourcesOccupancy).Where(isBuildable).ToList();
            var fullBodyMinerals = mineralTopLefts.SelectMany(x => ToFullBodyResource(x, MineralWidthTiles, MineralHeightTiles));
            var fullBodyGeysers = geyserTopLefts.SelectMany(x => ToFullBodyResource(x, GeyserWidthTiles, GeyserHeightTiles));

            var scoredBuildableTiles = buildableTiles.Select(tile => CalculateTileScore(tile, fullBodyMinerals, fullBodyGeysers)).ToDictionary(x => x.Tile, x => x.Score);

            return buildableTiles.Select(tile => CalculateBuildingPlacementScore(tile, scoredBuildableTiles))
                .OrderBy(x => x.Score)
                .First()
                .Tile;
        }

        private static (TilePosition Tile, double Score) CalculateBuildingPlacementScore(TilePosition tile, IReadOnlyDictionary<TilePosition, double> scoredBuildableTiles)
        {
            var xRange = Enumerable.Range(tile.X, MainBuildingWidthTiles);
            var yRange = Enumerable.Range(tile.Y, MainBuildingHeightTiles);
            var buildingBodyTiles = xRange.SelectMany(x => yRange.Select(y => new TilePosition(x, y))).ToList();

            if (!buildingBodyTiles.All(scoredBuildableTiles.ContainsKey))
            {
                return (tile, double.MaxValue);
            }

            var positionScore = buildingBodyTiles.Select(x => scoredBuildableTiles[x]).Sum();
            return (tile, positionScore);
        }

        internal static (TilePosition Tile, double Score) CalculateTileScore(TilePosition tile, IEnumerable<TilePosition> mineralTiles, IEnumerable<TilePosition> geyserTiles)
        {
            var score = mineralTiles.Concat(geyserTiles).Select(resource => Distance(resource, tile)).Average();

            return (tile, score);
        }

        private static double Distance(TilePosition left, TilePosition right)
        {
            return Distances.Euclidean(left, right);
        }

        private static IEnumerable<TilePosition> YieldSearchBoxTiles(int searchBoxMinX, int searchBoxMaxX, int searchBoxMinY, int searchBoxMaxY)
        {
            for (var x = searchBoxMinX; x <= searchBoxMaxX; x++)
                for (var y = searchBoxMinY; y <= searchBoxMaxY; y++)
                    yield return new TilePosition(x, y);
        }

        internal static IEnumerable<TilePosition> ToFullBodyResource(TilePosition resourceTopLeft, int resourceWidth, int resourceHeight)
        {
            for (var x = resourceTopLeft.X; x <= resourceTopLeft.X + resourceWidth - 1; x++)
                for (var y = resourceTopLeft.Y; y <= resourceTopLeft.Y + resourceHeight - 1; y++)
                    yield return new TilePosition(x, y);
        }

        internal static IEnumerable<TilePosition> ToResourceOccupancy(TilePosition resourceTopLeft, int resourceWidth, int resourceHeight)
        {
            for (var x = resourceTopLeft.X - MinDistanceFromResourceTiles; x <= resourceTopLeft.X + resourceWidth - 1 + MinDistanceFromResourceTiles; x++)
                for (var y = resourceTopLeft.Y - MinDistanceFromResourceTiles; y <= resourceTopLeft.Y + resourceHeight - 1 + MinDistanceFromResourceTiles; y++)
                {
                    yield return new TilePosition(x, y);
                }
        }

        internal int GetMinXWithinMapBoundary(int potentialMinX) => potentialMinX < 0 ? 0 : potentialMinX;
        internal int GetMinYWithinMapBoundary(int potentialMinY) => potentialMinY < 0 ? 0 : potentialMinY;

        internal int GetMaxXWithinMapBoundary(int potentialMaxX) => potentialMaxX >= _mapWidthTiles ? _mapWidthTiles - 1 : potentialMaxX;
        internal int GetMaxYWithinMapBoundary(int potentialMaxY) => potentialMaxY >= _mapHeightTiles ? _mapHeightTiles - 1 : potentialMaxY;
    }
}