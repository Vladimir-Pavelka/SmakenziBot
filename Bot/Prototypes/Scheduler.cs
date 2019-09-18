namespace SmakenziBot.Prototypes
{
    using BroodWar.Api.Enum;
    using SmakenziBot.BuildOrder.Steps;
    using System.Collections.Generic;

    public class Scheduler
    {
        private readonly List<MacroTask> _schedule;
        private readonly List<Step> _opening;

        public Scheduler(List<MacroTask> schedule)
        {
            _schedule = schedule;
        }

        public void Execute()
        {
            // check game state, scheduled tasks, executing tasks, if should schedule
            _schedule.Add(new MacroTask());
        }

        private readonly IReadOnlyCollection<UnitType> _gameStartSituation =
            new[]
            {
                UnitType.Zerg_Hatchery, UnitType.Zerg_Overlord,
                UnitType.Zerg_Drone, UnitType.Zerg_Drone,
                UnitType.Zerg_Drone, UnitType.Zerg_Drone
            };

        //private Step GetNextStep()
        //{

        //    foreach (var step in _opening)
        //    {
                
        //    }
        //}
    }
}