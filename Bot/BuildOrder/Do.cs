namespace SmakenziBot.BuildOrder
{
    using Prerequisities;
    using Steps;

    public static class Do
    {
        public static BoAction SendDroneToNatural => new BoAction(ActionType.MoveDroneToNatural, new ResourcePrerequisite(170, 0));
        public static BoAction SendDroneToThird => new BoAction(ActionType.MoveDroneToThird);
    }
}