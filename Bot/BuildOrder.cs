namespace SmakenziBot
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public static class Steps
    {
        public static MorphUnitStep Drone => new MorphUnitStep(UnitType.Zerg_Drone, new ResourcePrerequisite(50, 0));
        public static MorphUnitStep Overlord => new MorphUnitStep(UnitType.Zerg_Overlord, new ResourcePrerequisite(100, 0));
        public static MorphUnitStep Zergling => new MorphUnitStep(UnitType.Zerg_Zergling, new ResourcePrerequisite(50, 0), new BuildingPrerequisite(UnitType.Zerg_Spawning_Pool));
        public static ConstructBuildingStep Hatchery => new ConstructBuildingStep(UnitType.Zerg_Hatchery, new ResourcePrerequisite(300, 0));
        public static ConstructBuildingStep SpawningPool => new ConstructBuildingStep(UnitType.Zerg_Spawning_Pool, new ResourcePrerequisite(200, 0));
    }

    public class BuildOrder
    {
        private static IEnumerable<Step> GetSteps()
        {
            for (var x = 0; x < 5; x++) yield return Steps.Drone;
            yield return Steps.Overlord;
            for (var x = 0; x < 3; x++) yield return Steps.Drone;
            yield return Steps.Hatchery;
            yield return Steps.SpawningPool;
            for (var x = 0; x < 3; x++) yield return Steps.Drone;
            yield return Steps.Hatchery;
            for (var x = 0; x < 3; x++) yield return Steps.Zergling;
            yield return Steps.Overlord;
            for (var x = 0; x < 5; x++) yield return Steps.Zergling;
            yield return Steps.Overlord;
            for (var x = 0; x < 18; x++) yield return Steps.Zergling;
            yield return Steps.Overlord;
            foreach (var unused in Enumerable.Range(0, int.MaxValue)) yield return Steps.Zergling;
        }

        private static readonly IEnumerator<Step> StepEnumerator = GetSteps().GetEnumerator();

        public Step Current
        {
            get
            {
                while (StepEnumerator.Current == null) StepEnumerator.MoveNext();
                if (StepEnumerator.Current.IsCompleted) StepEnumerator.MoveNext();
                return StepEnumerator.Current;
            }
        }
    }

    public class Step
    {
        protected IEnumerable<Prerequisite> Prerequisites = Enumerable.Empty<Prerequisite>();

        public UnitType Item { get; protected set; }

        public bool AllPrerequisitesMet() => Prerequisites.All(x => x.IsMet());

        public bool IsCompleted { get; private set; }

        public void Complete()
        {
            IsCompleted = true;
        }
    }

    public class MorphUnitStep : Step
    {
        private static readonly IDictionary<UnitType, int> _requiredSupply = new Dictionary<UnitType, int>
        {
            { UnitType.Zerg_Overlord, 0},
            { UnitType.Zerg_Drone, 1},
            { UnitType.Zerg_Zergling, 1}
        };

        public MorphUnitStep(UnitType item, params Prerequisite[] prerequisites) : this(item, prerequisites.ToList())
        {
        }

        public MorphUnitStep(UnitType item, IEnumerable<Prerequisite> prerequisites)
        {
            var defaultPrerequisites = new Prerequisite[]
                {new UnitExistsPrerequisite(UnitType.Zerg_Larva), new AvailableSupplyPrerequisite(_requiredSupply[item])};

            Prerequisites = defaultPrerequisites.Concat(prerequisites).ToList();
            Item = item;
        }
    }

    public class ConstructBuildingStep : Step
    {
        public ConstructBuildingStep(UnitType item, params Prerequisite[] prerequisites) : this(item, prerequisites.ToList())
        {
        }

        public ConstructBuildingStep(UnitType item, IEnumerable<Prerequisite> prerequisites)
        {
            var defaultPrerequisites = new Prerequisite[]
                {new UnitExistsPrerequisite(UnitType.Zerg_Drone)};

            Prerequisites = defaultPrerequisites.Concat(prerequisites).ToList();
            Item = item;
        }
    }

    public abstract class Prerequisite
    {
        public abstract bool IsMet();
    }

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

    public class UnitExistsPrerequisite : Prerequisite
    {
        private readonly UnitType _unitType;

        public UnitExistsPrerequisite(UnitType unitType)
        {
            _unitType = unitType;
        }

        public override bool IsMet() => Game.Self.Units.Any(x => x.UnitType.Type == _unitType);

    }

    public class AvailableSupplyPrerequisite : Prerequisite
    {
        private readonly int _availableSupply;

        public AvailableSupplyPrerequisite(int availableSupply)
        {
            _availableSupply = availableSupply;
        }

        public override bool IsMet() => (Game.Self.SupplyTotal - Game.Self.SupplyUsed) / 2 >= _availableSupply;
    }

    public class CurrentSupplyPrerequisite : Prerequisite
    {
        private readonly int _currentSupply;

        public CurrentSupplyPrerequisite(int currentSupply)
        {
            _currentSupply = currentSupply;
        }

        public override bool IsMet() => Game.Self.SupplyUsed / 2 >= _currentSupply;
    }

    public class MaxSupplyPrerequisite : Prerequisite
    {
        private readonly int _maxSupply;

        public MaxSupplyPrerequisite(int maxSupply)
        {
            _maxSupply = maxSupply;
        }

        public override bool IsMet() => Game.Self.SupplyTotal / 2 >= _maxSupply;
    }

    public class BuildingPrerequisite : Prerequisite
    {
        private readonly UnitType _buildingPrerequisite;

        public BuildingPrerequisite(UnitType buildingType)
        {
            _buildingPrerequisite = buildingType;
        }

        public override bool IsMet() => Game.Self.Units.Where(x => x.UnitType.IsBuilding).Where(x => x.IsCompleted).Any(x => x.UnitType.Type == _buildingPrerequisite);
    }
}