using System.Collections.Generic;

namespace SmakenziBot.Prototypes
{
    public class SchedulerService
    {
        private readonly List<Scheduler> _activeSchedulers = new List<Scheduler>();

        public void Execute()
        {
            _activeSchedulers.ForEach(x => x.Execute());
        }
    }
}