namespace SmakenziBot.BuildOrder.Prerequisities
{
    using BroodWar.Api;

    public class ResourcePrerequisite : Prerequisite
    {
        public int Minerals { get; }
        public int Gas { get; }

        public ResourcePrerequisite(int minerals, int gas)
        {
            Minerals = minerals;
            Gas = gas;
        }

        public override bool IsMet() => Game.Self.Minerals >= Minerals && Game.Self.Gas >= Gas;
    }
}