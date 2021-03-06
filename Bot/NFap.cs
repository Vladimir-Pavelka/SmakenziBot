﻿namespace SmakenziBot
{
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using Utils;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class NFap
    {
        private readonly IList<Unit> _player1 = new List<Unit>();
        private readonly IList<Unit> _player2 = new List<Unit>();
        private bool _didSomething;

        public void AddIfCombatUnit(Unit u, IList<Unit> list)
        {
            if (u.Is(UnitType.Protoss_Interceptor)) return;
            if (u.UnitType.CanAttack || u.Is(UnitType.Terran_Medic)) list.Add(u);
        }

        public void Simulate(int nFrames)
        {
            while (nFrames > 0)
            {
                if (!_player1.Any() || !_player2.Any()) break;

                _didSomething = false;
                SimulateInner();

                if (!_didSomething) break;
                nFrames--;
            }
        }

        public void SimulateInner()
        {
        }

        public int Score(Unit u)
        {
            if (u.HitPoints == 0 || u.InitialHitPoints == 0) return 0;
            //return ((fu.score * fu.health) / (fu.maxHealth * 2)) +
            //       (fu.unitType == BWAPI::UnitTypes::Terran_Bunker) * BWAPI::UnitTypes::Terran_Marine.destroyScore() * 4;
            return 0;
        }
    }
}

/*
std::pair<int, int> FastAPproximation::playerScores() const {
std::pair<int, int> res;

for (const auto &u : player1)
res.first += score(u);

for (const auto &u : player2)
res.second += score(u);

return res;
}

std::pair<int, int> FastAPproximation::playerScoresUnits() const {
std::pair<int, int> res;

for (const auto &u : player1)
if (!u.unitType.isBuilding())
  res.first += score(u);

for (const auto &u : player2)
if (!u.unitType.isBuilding())
  res.second += score(u);

return res;
}

std::pair<int, int> FastAPproximation::playerScoresBuildings() const {
std::pair<int, int> res;

for (const auto &u : player1)
if (u.unitType.isBuilding())
  res.first += score(u);

for (const auto &u : player2)
if (u.unitType.isBuilding())
  res.second += score(u);

return res;
}

std::pair<std::vector<FastAPproximation::FAPUnit> *, std::vector<FastAPproximation::FAPUnit> *> FastAPproximation::getState() {
return {&player1, &player2};
}

void FastAPproximation::clear() { player1.clear(), player2.clear(); }

void FastAPproximation::dealDamage(const FastAPproximation::FAPUnit &fu, int damage, BWAPI::DamageType const damageType) {
damage <<= 8;
auto const remainingShields = fu.shields - damage + (fu.shieldArmor << 8);
if (remainingShields > 0) {
fu.shields = remainingShields;
return;
} else if (fu.shields) {
damage -= fu.shields + (fu.shieldArmor << 8);
fu.shields = 0;
}

if (!damage)
return;

damage -= fu.armor << 8;

if (damageType == BWAPI::DamageTypes::Concussive) {
if (fu.unitSize == BWAPI::UnitSizeTypes::Large)
  damage = damage / 4;
else if (fu.unitSize == BWAPI::UnitSizeTypes::Medium)
  damage = damage / 2;
} else if (damageType == BWAPI::DamageTypes::Explosive) {
if (fu.unitSize == BWAPI::UnitSizeTypes::Small)
  damage = damage / 2;
else if (fu.unitSize == BWAPI::UnitSizeTypes::Medium)
  damage = (damage * 3) / 4;
}

fu.health -= MAX(128, damage);
}

int inline FastAPproximation::distButNotReally(const FAPUnit &u1, const FAPUnit &u2) {
return (u1.x - u2.x) * (u1.x - u2.x) + (u1.y - u2.y) * (u1.y - u2.y);
}

bool FastAPproximation::isSuicideUnit(BWAPI::UnitType const ut) {
return (ut == BWAPI::UnitTypes::Zerg_Scourge ||
      ut == BWAPI::UnitTypes::Terran_Vulture_Spider_Mine ||
      ut == BWAPI::UnitTypes::Zerg_Infested_Terran ||
      ut == BWAPI::UnitTypes::Protoss_Scarab);
}

void FastAPproximation::unitsim(const FAPUnit &fu, std::vector<FAPUnit> &enemyUnits) {
if (fu.attackCooldownRemaining) {
didSomething = true;
return;
}

auto closestEnemy = enemyUnits.end();
int closestDist;

for (auto enemyIt = enemyUnits.begin(); enemyIt != enemyUnits.end();
   ++enemyIt) {
if (enemyIt->flying) {
  if (fu.airDamage) {
    auto const d = distButNotReally(fu, *enemyIt);
    if ((closestEnemy == enemyUnits.end() || d < closestDist) &&
        d >= fu.airMinRange) {
      closestDist = d;
      closestEnemy = enemyIt;
    }
  }
} else {
  if (fu.groundDamage) {
    auto const d = distButNotReally(fu, *enemyIt);
    if ((closestEnemy == enemyUnits.end() || d < closestDist) &&
        d >= fu.groundMinRange) {
      closestDist = d;
      closestEnemy = enemyIt;
    }
  }
}
}

if (closestEnemy != enemyUnits.end() && sqrt(closestDist) <= fu.speed &&
  !(fu.x == closestEnemy->x && fu.y == closestEnemy->y)) {
fu.x = closestEnemy->x;
fu.y = closestEnemy->y;
closestDist = 0;

didSomething = true;
}

if (closestEnemy != enemyUnits.end() &&
  closestDist <=
      (closestEnemy->flying ? fu.groundMaxRange : fu.airMinRange)) {
if (closestEnemy->flying)
  dealDamage(*closestEnemy, fu.airDamage, fu.airDamageType),
      fu.attackCooldownRemaining = fu.airCooldown;
else {
  dealDamage(*closestEnemy, fu.groundDamage, fu.groundDamageType);
  fu.attackCooldownRemaining = fu.groundCooldown;
  if (fu.elevation != -1 && closestEnemy->elevation != -1)
    if (closestEnemy->elevation > fu.elevation)
      fu.attackCooldownRemaining += fu.groundCooldown;
}

if (closestEnemy->health < 1) {
  auto const temp = *closestEnemy;
  *closestEnemy = enemyUnits.back();
  enemyUnits.pop_back();
  unitDeath(temp, enemyUnits);
}

didSomething = true;
} else if (closestEnemy != enemyUnits.end() && sqrt(closestDist) > fu.speed) {
auto const dx = closestEnemy->x - fu.x;
auto const dy = closestEnemy->y - fu.y;

fu.x += static_cast<int>(dx * (fu.speed / sqrt(dx * dx + dy * dy)));
fu.y += static_cast<int>(dy * (fu.speed / sqrt(dx * dx + dy * dy)));

didSomething = true;
}
}

void FastAPproximation::medicsim(const FAPUnit &fu, std::vector<FAPUnit> &friendlyUnits) {
auto closestHealable = friendlyUnits.end();
int closestDist;

for (auto it = friendlyUnits.begin(); it != friendlyUnits.end(); ++it) {
if (it->isOrganic && it->health < it->maxHealth && !it->didHealThisFrame) {
  auto const d = distButNotReally(fu, *it);
  if (closestHealable == friendlyUnits.end() || d < closestDist) {
    closestHealable = it;
    closestDist = d;
  }
}
}

if (closestHealable != friendlyUnits.end()) {
fu.x = closestHealable->x;
fu.y = closestHealable->y;

closestHealable->health += 150;

if (closestHealable->health > closestHealable->maxHealth)
  closestHealable->health = closestHealable->maxHealth;

closestHealable->didHealThisFrame = true;
}
}

bool FastAPproximation::suicideSim(const FAPUnit &fu, std::vector<FAPUnit> &enemyUnits) {
auto closestEnemy = enemyUnits.end();
int closestDist;

for (auto enemyIt = enemyUnits.begin(); enemyIt != enemyUnits.end(); ++enemyIt) {
if (enemyIt->flying) {
  if (fu.airDamage) {
    auto const d = distButNotReally(fu, *enemyIt);
    if ((closestEnemy == enemyUnits.end() || d < closestDist) &&
        d >= fu.airMinRange) {
      closestDist = d;
      closestEnemy = enemyIt;
    }
  }
} else {
  if (fu.groundDamage) {
    int d = distButNotReally(fu, *enemyIt);
    if ((closestEnemy == enemyUnits.end() || d < closestDist) &&
        d >= fu.groundMinRange) {
      closestDist = d;
      closestEnemy = enemyIt;
    }
  }
}
}

if (closestEnemy != enemyUnits.end() && sqrt(closestDist) <= fu.speed) {
if (closestEnemy->flying)
  dealDamage(*closestEnemy, fu.airDamage, fu.airDamageType);
else
  dealDamage(*closestEnemy, fu.groundDamage, fu.groundDamageType);

if (closestEnemy->health < 1) {
  auto const temp = *closestEnemy;
  *closestEnemy = enemyUnits.back();
  enemyUnits.pop_back();
  unitDeath(temp, enemyUnits);
}

didSomething = true;
return true;
} else if (closestEnemy != enemyUnits.end() && sqrt(closestDist) > fu.speed) {
auto const dx = closestEnemy->x - fu.x;
auto const dy = closestEnemy->y - fu.y;

fu.x += static_cast<int>(dx * (fu.speed / sqrt(dx * dx + dy * dy)));
fu.y += static_cast<int>(dy * (fu.speed / sqrt(dx * dx + dy * dy)));

didSomething = true;
}

return false;
}

void FastAPproximation::isimulate() {
const auto simUnit = [this](auto &unit, auto &friendly, auto &enemy) {
if(isSuicideUnit(unit->unitType)) {
  auto const unitDied = suicideSim(*unit, enemy);
  if (unitDied)
    unit = friendly.erase(unit);
  else ++unit;
} else {
  if (unit->unitType == BWAPI::UnitTypes::Terran_Medic)
    medicsim(*unit, friendly);
  else
    unitsim(*unit, enemy);
  ++unit;
}
};

for (auto fu = player1.begin(); fu != player1.end();) {
simUnit(fu, player1, player2);
}

for (auto fu = player2.begin(); fu != player2.end();) {
simUnit(fu, player2, player1);
}

const auto updateUnit = [](FAPUnit &fu) {
if (fu.attackCooldownRemaining)
  --fu.attackCooldownRemaining;
if (fu.didHealThisFrame)
  fu.didHealThisFrame = false;

if (fu.unitType.getRace() == BWAPI::Races::Zerg) {
  if (fu.health < fu.maxHealth)
    fu.health += 4;
  if (fu.health > fu.maxHealth)
    fu.health = fu.maxHealth;
}
else if (fu.unitType.getRace() == BWAPI::Races::Protoss) {
  if (fu.shields < fu.maxShields)
    fu.shields += 7;
  if (fu.shields > fu.maxShields)
    fu.shields = fu.maxShields;
}
};

for (auto &fu : player1)
updateUnit(fu);

for (auto &fu : player2)
updateUnit(fu);
}

void FastAPproximation::unitDeath(const FAPUnit &fu, std::vector<FAPUnit> &itsFriendlies) {
if (fu.unitType == BWAPI::UnitTypes::Terran_Bunker) {
convertToUnitType(fu, BWAPI::UnitTypes::Terran_Marine);

for (unsigned i = 0; i < 4; ++i)
  itsFriendlies.push_back(fu);
}
}

void FastAPproximation::convertToUnitType(const FAPUnit &fu, BWAPI::UnitType const ut) {
EnemyData ed;
ed.lastPosition = {fu.x, fu.y};
ed.lastPlayer = fu.player;
ed.lastType = ut;

FAPUnit funew(ed);
funew.attackCooldownRemaining = fu.attackCooldownRemaining;
funew.elevation = fu.elevation;

fu = funew;
}

FastAPproximation::FAPUnit::FAPUnit(BWAPI::Unit const u) : FAPUnit(EnemyData(u)) {}

FastAPproximation::FAPUnit::FAPUnit(EnemyData ed)
: x(ed.lastPosition.x), y(ed.lastPosition.y),

  health(ed.expectedHealth()),

  maxHealth(ed.lastType.maxHitPoints()), armor(ed.lastPlayer->armor(ed.lastType)),

  shields(ed.expectedShields()),
  shieldArmor(ed.lastPlayer->getUpgradeLevel(BWAPI::UpgradeTypes::Protoss_Plasma_Shields)),
  maxShields(ed.lastType.maxShields()),
  speed(ed.lastPlayer->topSpeed(ed.lastType)), flying(ed.lastType.isFlyer()),

  groundDamage(ed.lastPlayer->damage(ed.lastType.groundWeapon())),
  groundCooldown(ed.lastType.groundWeapon().damageFactor() &&
                 ed.lastType.maxGroundHits()
                   ? ed.lastPlayer->weaponDamageCooldown(ed.lastType) /
                     (ed.lastType.groundWeapon().damageFactor() *
                      ed.lastType.maxGroundHits())
                   : 0),
  groundMaxRange(ed.lastPlayer->weaponMaxRange(ed.lastType.groundWeapon())),
  groundMinRange(ed.lastType.groundWeapon().minRange()),
  groundDamageType(ed.lastType.groundWeapon().damageType()),

  airDamage(ed.lastPlayer->damage(ed.lastType.airWeapon())),
  airCooldown(ed.lastType.airWeapon().damageFactor() &&
              ed.lastType.maxAirHits()
                ? ed.lastType.airWeapon().damageCooldown() /
                  (ed.lastType.airWeapon().damageFactor() *
                   ed.lastType.maxAirHits())
                : 0),
  airMaxRange(ed.lastPlayer->weaponMaxRange(ed.lastType.airWeapon())),
  airMinRange(ed.lastType.airWeapon().minRange()),
  airDamageType(ed.lastType.airWeapon().damageType()),

  unitType(ed.lastType), player(ed.lastPlayer),
  unitSize(ed.lastType.size()),
  isOrganic(ed.lastType.isOrganic()), score(ed.lastType.destroyScore()) {

static auto nextId = 0;
id = nextId++;

switch (ed.lastType) {
case BWAPI::UnitTypes::Protoss_Carrier:
groundDamage = ed.lastPlayer->damage(
    BWAPI::UnitTypes::Protoss_Interceptor.groundWeapon());

if (ed.u && ed.u->isVisible()) {
  auto const interceptorCount = ed.u->getInterceptorCount();
  if (interceptorCount) {
    groundCooldown = static_cast<int>(round(37.0f / interceptorCount));
  } else {
    groundDamage = 0;
    groundCooldown = 5;
  }
} else {
  if (ed.lastPlayer) {
    groundCooldown =
        static_cast<int>(round(37.0f / (ed.lastPlayer->getUpgradeLevel(BWAPI::UpgradeTypes::Carrier_Capacity) ? 8 : 4)));
  } else {
    groundCooldown = static_cast<int>(round(37.0f / 8));
  }
}

groundDamageType = BWAPI::UnitTypes::Protoss_Interceptor.groundWeapon().damageType();
groundMaxRange = 32 * 8;

airDamage = groundDamage;
airDamageType = groundDamageType;
airCooldown = groundCooldown;
airMaxRange = groundMaxRange;
break;

case BWAPI::UnitTypes::Terran_Bunker:
groundDamage = ed.lastPlayer->damage(BWAPI::WeaponTypes::Gauss_Rifle);
groundCooldown = BWAPI::UnitTypes::Terran_Marine.groundWeapon().damageCooldown() / 4;
groundMaxRange = ed.lastPlayer->weaponMaxRange(BWAPI::UnitTypes::Terran_Marine.groundWeapon()) + 32;

airDamage = groundDamage;
airCooldown = groundCooldown;
airMaxRange = groundMaxRange;
break;

case BWAPI::UnitTypes::Protoss_Reaver:
groundDamage = ed.lastPlayer->damage(BWAPI::WeaponTypes::Scarab);
break;
}

if (ed.u && ed.u->isStimmed()) {
groundCooldown /= 2;
airCooldown /= 2;
}

if (ed.u && ed.u->isVisible() && !ed.u->isFlying())
elevation = BWAPI::Broodwar->getGroundHeight(ed.u->getTilePosition());

groundMaxRange *= groundMaxRange;
groundMinRange *= groundMinRange;
airMaxRange *= airMaxRange;
airMinRange *= airMinRange;

health <<= 8;
maxHealth <<= 8;
shields <<= 8;
maxShields <<= 8;
}

const FastAPproximation::FAPUnit &FastAPproximation::FAPUnit::
operator=(const FAPUnit &other) const {
x = other.x, y = other.y;
health = other.health, maxHealth = other.maxHealth;
shields = other.shields, maxShields = other.maxShields;
speed = other.speed, armor = other.armor, flying = other.flying,
unitSize = other.unitSize;
groundDamage = other.groundDamage, groundCooldown = other.groundCooldown,
groundMaxRange = other.groundMaxRange, groundMinRange = other.groundMinRange,
groundDamageType = other.groundDamageType;
airDamage = other.airDamage, airCooldown = other.airCooldown,
airMaxRange = other.airMaxRange, airMinRange = other.airMinRange,
airDamageType = other.airDamageType;
score = other.score;
attackCooldownRemaining = other.attackCooldownRemaining;
unitType = other.unitType;
isOrganic = other.isOrganic;
didHealThisFrame = other.didHealThisFrame;
elevation = other.elevation;
player = other.player;

return *this;
}

bool FastAPproximation::FAPUnit::operator<(const FAPUnit &other) const {
return id < other.id;
}

} */
