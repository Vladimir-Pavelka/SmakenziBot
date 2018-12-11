namespace SmakenziBot.BuildOrder.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using BroodWar.Api.Enum;
    using Prerequisities;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class ResearchUpgradeStep : Step<UpgradeType>
    {
        public ResearchUpgradeStep(UpgradeType item, params Prerequisite[] prerequisites) : this(item, prerequisites.ToList())
        {
        }

        public ResearchUpgradeStep(UpgradeType item, IEnumerable<Prerequisite> prerequisites)
        {
            Research = Upgrade.AllUpgrades.First(u => u.TypeEquals(item));
            ResearchedBy = Research.WhatUpgrades.Type;
            
            var defaultPrerequisites = new Prerequisite[]
                {new UnitExistsPrerequisite(ResearchedBy)};

            Prerequisites = defaultPrerequisites.Concat(prerequisites).ToList();
            Item = item;
        }

        public UnitType ResearchedBy { get; }
        public Upgrade Research { get; }
    }
}