namespace SmakenziBot.BuildOrder
{
    using System.Collections.Generic;
    using Steps;

    public static class Openings
    {
        public static IEnumerable<Step> NinePool()
        {
            for (var x = 0; x < 5; x++) yield return Make.Drone;
            yield return Make.SpawningPool;
            yield return Make.Drone;
            yield return Make.Extractor;
            yield return Make.Drone;
            yield return Make.CancelExtractor;
            yield return Make.Overlord;
        }

        public static IEnumerable<Step> OverPool()
        {
            for (var x = 0; x < 5; x++) yield return Make.Drone;
            yield return Make.Overlord;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.SpawningPool;
        }

        public static IEnumerable<Step> TwelveHatch()
        {
            for (var x = 0; x < 5; x++) yield return Make.Drone;
            yield return Make.Overlord;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.Drone;
            yield return Make.Hatchery(HatcheryType.NaturalExp);
            yield return Make.SpawningPool;
        }
    }
}