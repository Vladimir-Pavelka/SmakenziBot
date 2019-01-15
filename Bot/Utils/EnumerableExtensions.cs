namespace SmakenziBot.Utils
{
    using System;
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static TElem MinBy<TElem>(this IEnumerable<TElem> source, Func<TElem, IComparable> selector)
        {
            var hasAny = false;
            var minElem = default(TElem);
            IComparable minKey = null;

            foreach (var elem in source)
            {
                var projected = selector(elem);
                if (hasAny && projected.CompareTo(minKey) >= 0) continue;
                minElem = elem;
                hasAny = true;
                minKey = projected;
            }

            if (!hasAny) throw new InvalidOperationException("Sequence contains no elements");
            return minElem;
        }

        public static TElem MaxBy<TElem>(this IEnumerable<TElem> source, Func<TElem, IComparable> selector)
        {
            var hasAny = false;
            var maxElem = default(TElem);
            IComparable maxKey = null;

            foreach (var elem in source)
            {
                var projected = selector(elem);
                if (hasAny && projected.CompareTo(maxKey) <= 0) continue;
                maxElem = elem;
                hasAny = true;
                maxKey = projected;
            }

            if (!hasAny) throw new InvalidOperationException("Sequence contains no elements");
            return maxElem;
        }
    }
}