namespace SmakenziBot.BuildOrder
{
    using System.Collections.Generic;
    using Steps;

    public interface IBuildOrder
    {
        IEnumerable<Step> GetSteps();
    }
}