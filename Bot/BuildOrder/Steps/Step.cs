namespace SmakenziBot.BuildOrder.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using Prerequisities;

    public class Step<TItem> : Step
    {
        public TItem Item { get; protected set; }
        public override string ToString() => Item.ToString();
    }

    public class Step
    {
        protected IEnumerable<Prerequisite> Prerequisites = Enumerable.Empty<Prerequisite>();
        public bool AllPrerequisitesMet() => Prerequisites.All(x => x.IsMet());
        public bool IsCompleted { get; private set; }

        public void Complete()
        {
            IsCompleted = true;
        }
    }
}