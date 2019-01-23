namespace SmakenziBot.BuildOrder.Steps
{
    using Prerequisities;

    public class BoAction : Step
    {
        public ActionType ActionType { get; }

        public BoAction(ActionType actionType, params Prerequisite[] prerequisities)
        {
            ActionType = actionType;
            Prerequisites = prerequisities;
        }

        public override string ToString() => $"{ActionType}";
    }
}