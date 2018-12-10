namespace SmakenziBot
{
    using System;
    using BroodWar.Api;

    public static class Distances
    {
        public static double Euclidean(TilePosition left, TilePosition right)
        {
            var xDistance = left.X - right.X;
            var yDistance = left.Y - right.Y;

            return Math.Sqrt(xDistance * xDistance + yDistance * yDistance);
        }

        public static double EuclideanSquared(TilePosition left, TilePosition right)
        {
            var xDistance = left.X - right.X;
            var yDistance = left.Y - right.Y;

            return xDistance * xDistance + yDistance * yDistance;
        }

        public static double Diagonal(TilePosition left, TilePosition right)
        {
            const int straightCost = 10;
            const int diagonalCost = 14;

            var xDistance = Math.Abs(left.X - right.X);
            var yDistance = Math.Abs(left.Y - right.Y);

            return xDistance > yDistance
                ? straightCost * (xDistance - yDistance) + diagonalCost * yDistance
                : straightCost * (yDistance - xDistance) + diagonalCost * xDistance;
        }

        public static double Manhattan(TilePosition left, TilePosition right)
        {
            var xDistance = Math.Abs(left.X - right.X);
            var yDistance = Math.Abs(left.Y - right.Y);

            return xDistance + yDistance;
        }
    }
}