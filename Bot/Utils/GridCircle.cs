namespace SmakenziBot.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class GridCircle
    {
        public static IEnumerable<(int x, int y)> YieldIndices((int x, int y) center, int radius)
        {
            GuardArgsValid(center, radius);
            var numPoints = radius * 6;
            var step = Math.PI * 2 / numPoints;

            return Enumerable.Range(0, numPoints).Select(x => x * step)
                .Select(rad => (x: Math.Cos(rad) * radius, y: Math.Sin(rad) * radius))
                .Select(p => (x: (int)Math.Round(p.x), y: (int)Math.Round(p.y)))
                .Select(p => (p.x + center.x, p.y + center.y));
        }

        private static void GuardArgsValid((int x, int y) center, double radius)
        {
            if (center.x < 0 || center.y < 0) throw new ArgumentException($"Center {center} was invalid", nameof(center));
            if (radius <= 0) throw new ArgumentException($"Radius {radius} was invalid", nameof(radius));
        }
    }
}