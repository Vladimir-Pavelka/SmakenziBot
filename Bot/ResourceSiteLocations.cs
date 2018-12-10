namespace SmakenziBot
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;

    public class ResourceSiteLocations
    {
        private const int Treshold = 200;

        public static IReadOnlyCollection<HashSet<Unit>> Find(IEnumerable<Unit> resources)
        {
            var relevantResources = resources.Where(x => x.InitialResources > 200).ToList();
            return relevantResources.Cluster((a, b) => (a == b ? 0 : a.Distance(b)) < Treshold);
        }
    }
}