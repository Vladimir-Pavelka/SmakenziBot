namespace SmakenziBot.Utils
{
    using System;
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static TElem MinBy<TElem>(this IEnumerable<TElem> source, Func<TElem, IComparable> selector)
            where TElem : class
        {
            TElem minElem = null;
            IComparable minKey = null;

            foreach (var elem in source)
            {
                var projected = selector(elem);
                if (minElem != null && projected.CompareTo(minKey) >= 0) continue;
                minElem = elem;
                minKey = projected;
            }

            if (minElem == null) throw new InvalidOperationException("Sequence contains no elements");
            return minElem;
        }

        public static TElem MaxBy<TElem>(this IEnumerable<TElem> source, Func<TElem, IComparable> selector)
            where TElem : class
        {
            TElem maxElem = null;
            IComparable maxKey = null;

            foreach (var elem in source)
            {
                var projected = selector(elem);
                if (projected.CompareTo(maxKey) <= 0) continue;
                maxElem = elem;
                maxKey = projected;
            }

            if (maxElem == null) throw new InvalidOperationException("Sequence contains no elements");
            return maxElem;
        }
    }
}