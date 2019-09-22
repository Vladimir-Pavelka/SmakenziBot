namespace SmakenziBot.Prototypes
{
    using System;
    using SmakenziBot.BuildOrder.Steps;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class Scheduler
    {
        private readonly List<Step> _opening;
        private readonly IReadOnlyCollection<MacroTask> _queue;

        public Scheduler(IReadOnlyCollection<MacroTask> queue)
        {
            _queue = queue;
        }

        public MacroTask GetNext()
        {
            var nextStep = GetNextStep();
            return new MacroTask();
        }

        private readonly IReadOnlyCollection<UnitType> _gameStartSituation =
            new[]
            {
                UnitType.Zerg_Hatchery, UnitType.Zerg_Overlord,
                UnitType.Zerg_Drone, UnitType.Zerg_Drone,
                UnitType.Zerg_Drone, UnitType.Zerg_Drone
            };

        private Step GetNextStep()
        {
            var materialNow = Game.Self.Units.Select(x => x.UnitType);

            foreach (var step in _opening)
            {
                // if _gameStartSituation + sum(step) <= material now continue;
                return step;
            }

            throw new InvalidOperationException("No next step found");
        }
    }
}