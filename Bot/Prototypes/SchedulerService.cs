namespace SmakenziBot.Prototypes
{
    using System.Collections.Generic;
    using System.Linq;
    using NBWTA.Utils;

    public class SchedulerService
    {
        private readonly List<Scheduler> _activeSchedulers = new List<Scheduler>();
        private List<MacroTask> _queue;

        public SchedulerService(List<MacroTask> queue)
        {
            _queue = queue;
        }

        public void Update()
        {
            _activeSchedulers.Select(x => x.GetNext()).ForEach(_queue.Add);
        }
    }
}