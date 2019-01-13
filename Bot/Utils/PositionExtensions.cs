namespace SmakenziBot.Utils
{
    using BroodWar.Api;

    public static class PositionExtensions
    {
        private const int PixelsInWalkTile = 8;
        private const int PixelsInBuildTile = 32;
        private const int WalkTilesInBuildTile = 4;

        public static WalkPosition ToWalkTile(this Position p) =>
            new WalkPosition(p.X / PixelsInWalkTile, p.Y / PixelsInWalkTile);

        public static TilePosition ToBuildTile(this Position p) =>
            new TilePosition(p.X / PixelsInBuildTile, p.Y / PixelsInBuildTile);

        public static TilePosition ToBuildTile(this WalkPosition p) =>
            new TilePosition(p.X / WalkTilesInBuildTile, p.Y / WalkTilesInBuildTile);

        public static Position ToPixelTile(this WalkPosition p) =>
            new Position(p.X * PixelsInWalkTile, p.Y * PixelsInWalkTile);

        public static Position ToPixelTile(this TilePosition p) =>
            new Position(p.X * PixelsInBuildTile, p.Y * PixelsInBuildTile);

        public static WalkPosition ToWalkTile(this TilePosition p) =>
            new WalkPosition(p.X * WalkTilesInBuildTile, p.Y * WalkTilesInBuildTile);

        public static (int x, int y) AsTuple(this Position p) => (p.X, p.Y);
        public static (int x, int y) AsTuple(this WalkPosition p) => (p.X, p.Y);
        public static (int x, int y) AsTuple(this TilePosition p) => (p.X, p.Y);

        public static Position AsPixelTile(this (int x, int y) p) => new Position(p.x, p.y);
        public static WalkPosition AsWalkTile(this (int x, int y) p) => new WalkPosition(p.x, p.y);
        public static TilePosition AsBuildTile(this (int x, int y) p) => new TilePosition(p.x, p.y);
    }
}