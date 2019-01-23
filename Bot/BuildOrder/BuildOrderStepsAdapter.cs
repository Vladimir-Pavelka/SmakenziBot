namespace SmakenziBot.BuildOrder
{
    using System.Collections.Generic;
    using Steps;

    public class BuildOrderStepsAdapter
    {
        public BuildOrderStepsAdapter(IBuildOrder buildOrder)
        {
            _stepEnumerator = buildOrder.GetSteps().GetEnumerator();
            _stepEnumerator.MoveNext();
            _current = _stepEnumerator.Current;
        }

        private readonly IEnumerator<Step> _stepEnumerator;

        private Step _current;
        public (Step current, Step next) Get
        {
            get
            {
                while (_stepEnumerator.Current.IsCompleted) _stepEnumerator.MoveNext();
                if (_current.IsCompleted)
                {
                    _current = _stepEnumerator.Current;
                    _stepEnumerator.MoveNext();
                }
                return (_current, _stepEnumerator.Current);
            }
        }
    }
}