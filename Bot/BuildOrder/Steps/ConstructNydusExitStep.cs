namespace SmakenziBot.BuildOrder.Steps
{
    using Prerequisities;

    public class ConstructNydusExitStep : Step
    {
        public ConstructNydusExitStep()
        {
            Prerequisites = new[] { new NydusEntranceExistsPrerequisite() };
        }
    }
}