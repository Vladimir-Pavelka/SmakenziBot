namespace SmakenziBot.BuildOrder.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using Prerequisities;

    public class Step<TTarget> : Step
    {
        public TTarget Target { get; protected set; }
        public override string ToString() => Target.ToString();
    }

    public class Step
    {
        public IEnumerable<Prerequisite> Prerequisites { get; protected set; } = Enumerable.Empty<Prerequisite>();
        public bool AllPrerequisitesMet() => Prerequisites.All(x => x.IsMet());
        public bool IsCompleted { get; private set; }

        public void Complete()
        {
            IsCompleted = true;
        }
    }
}