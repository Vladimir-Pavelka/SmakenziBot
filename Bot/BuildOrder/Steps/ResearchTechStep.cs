namespace SmakenziBot.BuildOrder.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using BroodWar.Api.Enum;
    using Prerequisities;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class ResearchTechStep : Step<TechType>
    {
        public ResearchTechStep(TechType target, params Prerequisite[] prerequisites) : this(target,
            prerequisites.ToList())
        {
        }

        public ResearchTechStep(TechType target, IEnumerable<Prerequisite> extraPrerequisites)
        {
            Research = TechTypes.All[target];
            ResearchedBy = Research.WhatResearches.Type;

            var defaultPrerequisites = new Prerequisite[]
            {
                new BuildingExistsPrerequisite(ResearchedBy),
                new ResourcePrerequisite(Research.Price.Minerals, Research.Price.Gas)
            };

            Prerequisites = defaultPrerequisites.Concat(extraPrerequisites).ToList();
            Target = target;
        }

        public UnitType ResearchedBy { get; }
        public Tech Research { get; }
    }
}