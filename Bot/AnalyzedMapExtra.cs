﻿namespace SmakenziBot
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Result;
    using NBWTA.Utils;
    using Utils;

    public class AnalyzedMapExtra
    {
        private readonly AnalyzedMap _analyzedMap;

        public AnalyzedMapExtra(AnalyzedMap analyzedMap)
        {
            _analyzedMap = analyzedMap;

            var myStartLocation = Game.Self.StartLocation.ToWalkTile().AsTuple();
            var myMainBaseRegion = _analyzedMap.MapRegions.First(r => r.ContentTiles.Contains(myStartLocation));
            MyNaturals = myMainBaseRegion.AdjacentChokes.SelectMany(ch => ch.AdjacentRegions).Except(myMainBaseRegion.Yield())
                .Distinct()
                .Where(r => r.ResourceSites.Any())
                .ToList();

            MyStartRegion = myMainBaseRegion;

            ChokesBetweenMainAndNaturals = MyStartRegion.AdjacentChokes
                .Intersect(MyNaturals.SelectMany(n => n.AdjacentChokes)).ToList();

            NaturalsEntrances = MyNaturals.SelectMany(n => n.AdjacentChokes)
                .Except(ChokesBetweenMainAndNaturals).ToList();

            AllResourceRegions = _analyzedMap.MapRegions.Where(mr => mr.ResourceSites.Any()).ToList();
        }

        public MapRegion MyStartRegion { get; }
        public IReadOnlyCollection<MapRegion> MyNaturals { get; }
        public IReadOnlyCollection<ChokeRegion> ChokesBetweenMainAndNaturals { get; }
        public IReadOnlyCollection<ChokeRegion> NaturalsEntrances { get; }
        public IReadOnlyCollection<MapRegion> AllResourceRegions { get; }
    }
}