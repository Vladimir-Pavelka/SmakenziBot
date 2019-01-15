namespace SmakenziBot.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class UtilExtensions
    {
        public static (int x, int y) Average(this IReadOnlyCollection<(int x, int y)> points)
        {
            var avgX = (int)Math.Round(points.Average(t => t.x));
            var avgY = (int)Math.Round(points.Average(t => t.y));
            return (avgX, avgY);
        }
    }
}