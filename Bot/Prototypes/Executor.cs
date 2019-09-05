using System.Collections.Generic;

namespace SmakenziBot.Prototypes
{
    using System.Linq;

    public class Executor
    {
        private readonly List<MacroTask> _schedule;

        public Executor(List<MacroTask> schedule)
        {
            _schedule = schedule;
        }

        public void Execute()
        {
            if (!_schedule.Any()) return;
            // iterate trough _schedule, checking if can execute
        }
    }
}