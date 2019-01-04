namespace SmakenziBot.Behaviors.GameBehaviors
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;

    public class RememberEnemyBuildings : GameBehavior
    {
        public override void Execute()
        {
            if (Game.FrameCount % 20 != 0) return;
            var enemyBuildingsNoLongerThere = GameMemory.EnemyBuildings
                .Where(p => Game.IsVisible(p.X / 32, p.Y / 32))
                .Where(p => !Game.GetUnitsInRadius(p, 100).Any(u => IsEnemyBuilding(u) && u.Position == p))
                .ToList();

            enemyBuildingsNoLongerThere.ForEach(p => GameMemory.EnemyBuildings.Remove(p));

            VisibleEnemyBuildings.ForEach(b => GameMemory.EnemyBuildings.Add(b.Position));
        }

        public IEnumerable<Unit> VisibleEnemyBuildings =>
            Game.Enemy.Units.Where(u => u.UnitType.IsBuilding);

        public bool IsEnemyBuilding(Unit u) => Game.Enemy.Units.Contains(u) && u.UnitType.IsBuilding;
    }
}