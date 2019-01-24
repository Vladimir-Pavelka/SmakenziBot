namespace SmakenziBot.BuildOrder.Steps
{
    using System;

    [Flags]
    public enum ActionType
    {
        MoveDroneToNatural = 1,
        MoveDroneToThird = 2,
        MoveDroneToExpand = MoveDroneToNatural | MoveDroneToThird,
    }
}