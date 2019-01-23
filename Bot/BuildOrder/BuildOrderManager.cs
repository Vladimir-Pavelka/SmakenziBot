namespace SmakenziBot.BuildOrder
{
    using System;
    using Steps;

    public class BuildOrderManager
    {
        private readonly Func<Step, bool> _isScheduledOrExecuting;

        public BuildOrderManager(Func<Step, bool> isScheduledOrExecuting, Action<Step> schedule)
        {
            _isScheduledOrExecuting = isScheduledOrExecuting;
        }
    }
}

// ZvT
//12 - Hatchery @ expansion
//11 - Spawning Pool
//13 - Hatchery
//12 - Extractor
//@100% Spawning Pool - 2 Zerglings
//16 - Overlord
//@100 Gas:
//Lair
//Zergling Speed
//Extractor at expansion
//@100% Lair - Spire
//24 - Overlord
//25..27 - Drones
//28..33 Zerglings
//33 - Expansion or Sunken Colonies to adapt to the Terran builds
//@50% Spire - 3 Overlords
//Save Larvae
//33 - 9 Mutalisks
//@50 Gas - Hydralisk Den
//@100-200 Gas:
//2 more Mutalisks
//1 Overlord
//@75% Hydralisk Den - Evolution Chamber
//@200 Gas - Lurker Aspect
//@150 Gas - Carapace Upgrade