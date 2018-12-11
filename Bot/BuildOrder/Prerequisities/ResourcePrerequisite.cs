namespace SmakenziBot.BuildOrder.Prerequisities
{
    using BroodWar.Api;

    public class ResourcePrerequisite : Prerequisite
    {
        private readonly int _minerals;
        private readonly int _gas;

        public ResourcePrerequisite(int minerals, int gas)
        {
            _minerals = minerals;
            _gas = gas;
        }

        public override bool IsMet() => Game.Self.Minerals >= _minerals && Game.Self.Gas >= _gas;
    }
}