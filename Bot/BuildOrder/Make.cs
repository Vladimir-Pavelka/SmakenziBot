namespace SmakenziBot.BuildOrder
{
    using BroodWar.Api.Enum;
    using Steps;

    public static class Make
    {
        public static MorphUnitStep Drone => new MorphUnitStep(UnitType.Zerg_Drone);
        public static MorphUnitStep Overlord => new MorphUnitStep(UnitType.Zerg_Overlord);
        public static MorphUnitStep Zergling => new MorphUnitStep(UnitType.Zerg_Zergling);
        public static MorphUnitStep Hydralisk => new MorphUnitStep(UnitType.Zerg_Hydralisk);

        public static ConstructBuildingStep Hatchery => new ConstructBuildingStep(UnitType.Zerg_Hatchery);
        public static ConstructBuildingStep CreepColony => new ConstructBuildingStep(UnitType.Zerg_Creep_Colony);
        public static ConstructBuildingStep Extractor => new ConstructBuildingStep(UnitType.Zerg_Extractor);
        public static ConstructBuildingStep EvolutionChamber => new ConstructBuildingStep(UnitType.Zerg_Evolution_Chamber);
        public static ConstructBuildingStep SpawningPool => new ConstructBuildingStep(UnitType.Zerg_Spawning_Pool);
        public static ConstructBuildingStep HydraliskDen => new ConstructBuildingStep(UnitType.Zerg_Hydralisk_Den);

        public static MorphUpgradeBuildingStep Lair => new MorphUpgradeBuildingStep(UnitType.Zerg_Lair);
        public static MorphUpgradeBuildingStep SunkenColony => new MorphUpgradeBuildingStep(UnitType.Zerg_Sunken_Colony);

        public static CancelConstructBuildingStep CancelExtractor => new CancelConstructBuildingStep(UnitType.Zerg_Extractor);

        public static ResearchUpgradeStep MuscularAugments => new ResearchUpgradeStep(UpgradeType.Muscular_Augments);
        public static ResearchUpgradeStep GroovedSpines => new ResearchUpgradeStep(UpgradeType.Grooved_Spines);
        public static ResearchUpgradeStep MissileAttacks => new ResearchUpgradeStep(UpgradeType.Zerg_Missile_Attacks);
    }
}