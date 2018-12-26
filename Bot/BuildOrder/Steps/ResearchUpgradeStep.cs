namespace SmakenziBot.BuildOrder.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using BroodWar.Api.Enum;
    using Prerequisities;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class ResearchUpgradeStep : Step<UpgradeType>
    {
        public ResearchUpgradeStep(UpgradeType target, params Prerequisite[] prerequisites) : this(target, prerequisites.ToList())
        {
        }

        public ResearchUpgradeStep(UpgradeType target, IEnumerable<Prerequisite> extraPrerequisites)
        {
            Research = UpgradeTypes.All[target];
            ResearchedBy = Research.WhatUpgrades.Type;

            var defaultPrerequisites = new Prerequisite[]
            {
                new BuildingExistsPrerequisite(ResearchedBy),
                new ResourcePrerequisite(Research.MineralPrice(0), Research.GasPrice(0)), 
            };

            Prerequisites = defaultPrerequisites.Concat(extraPrerequisites).ToList();
            Target = target;
        }

        public UnitType ResearchedBy { get; }
        public Upgrade Research { get; }
    }
}