namespace SmakenziBot.BuildOrder
{
    using BroodWar.Api.Enum;
    using Steps;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public static class Make
    {
        public static MorphUnitStep Drone => new MorphUnitStep(UnitType.Zerg_Drone);
        public static MorphUnitStep Overlord => new MorphUnitStep(UnitType.Zerg_Overlord);
        public static MorphUnitStep Zergling => new MorphUnitStep(UnitType.Zerg_Zergling);
        public static MorphUnitStep Hydralisk => new MorphUnitStep(UnitType.Zerg_Hydralisk);
        public static MorphUnitStep Mutalisk => new MorphUnitStep(UnitType.Zerg_Mutalisk);

        //public static ConstructBuildingStep Hatchery => new ConstructBuildingStep(UnitType.Zerg_Hatchery, buildLocation);
        public static ConstructBuildingStep CreepColony => new ConstructBuildingStep(UnitType.Zerg_Creep_Colony);
        public static ConstructBuildingStep Extractor => new ConstructBuildingStep(UnitType.Zerg_Extractor);
        public static ConstructBuildingStep EvolutionChamber => new ConstructBuildingStep(UnitType.Zerg_Evolution_Chamber);
        public static ConstructBuildingStep SpawningPool => new ConstructBuildingStep(UnitType.Zerg_Spawning_Pool);
        public static ConstructBuildingStep HydraliskDen => new ConstructBuildingStep(UnitType.Zerg_Hydralisk_Den);
        public static ConstructBuildingStep Spire => new ConstructBuildingStep(UnitType.Zerg_Spire);
        public static ConstructBuildingStep QueensNest => new ConstructBuildingStep(UnitType.Zerg_Queens_Nest);
        public static ConstructBuildingStep DefilerMound => new ConstructBuildingStep(UnitType.Zerg_Defiler_Mound);
        public static ConstructBuildingStep UltraliskCavern => new ConstructBuildingStep(UnitType.Zerg_Ultralisk_Cavern);
        public static ConstructBuildingStep NydusCanal => new ConstructBuildingStep(UnitType.Zerg_Nydus_Canal);
        public static ConstructNydusExitStep NydusExit => new ConstructNydusExitStep();

        public static MorphUpgradeBuildingStep Lair => new MorphUpgradeBuildingStep(UnitType.Zerg_Lair);
        public static MorphUpgradeBuildingStep Hive => new MorphUpgradeBuildingStep(UnitType.Zerg_Hive);
        public static MorphUpgradeBuildingStep SunkenColony => new MorphUpgradeBuildingStep(UnitType.Zerg_Sunken_Colony);

        public static CancelConstructBuildingStep CancelExtractor => new CancelConstructBuildingStep(UnitType.Zerg_Extractor);

        public static ResearchUpgradeStep ZerglingSpeed => new ResearchUpgradeStep(UpgradeType.Metabolic_Boost);
        public static ResearchUpgradeStep HydraSpeed => new ResearchUpgradeStep(UpgradeType.Muscular_Augments);
        public static ResearchUpgradeStep HydraRange => new ResearchUpgradeStep(UpgradeType.Grooved_Spines);
        public static ResearchUpgradeStep MissileAttacks => new ResearchUpgradeStep(UpgradeType.Zerg_Missile_Attacks);

        public static ResearchTechStep LurkerAspect => new ResearchTechStep(TechType.Lurker_Aspect);

        public static ConstructBuildingStep Hatchery(HatcheryType type) => new HatcheryBuildingStep(type);
    }
}