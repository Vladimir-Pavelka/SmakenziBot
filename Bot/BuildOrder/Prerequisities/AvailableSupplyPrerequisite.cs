namespace SmakenziBot.BuildOrder.Prerequisities
{
    using BroodWar.Api;

    public class AvailableSupplyPrerequisite : Prerequisite
    {
        private readonly int _availableSupply;

        public AvailableSupplyPrerequisite(int availableSupply)
        {
            _availableSupply = availableSupply;
        }

        public override bool IsMet() => _availableSupply == 0 || (Game.Self.SupplyTotal - Game.Self.SupplyUsed) / 2 >= _availableSupply;
    }
}