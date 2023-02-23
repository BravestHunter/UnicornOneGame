using System;
using System.Collections.Generic;
using System.Linq;
using UnicornOne.Ecs.Other.Ability;
using UnicornOne.ScriptableObjects.Interfaces;

namespace UnicornOne.Ecs.Services
{
	public class AbilityService
	{
		private AbilitySet[] _abilitySets;

		public Dictionary<IHero, int> HeroToAbilitySetMap { get; } = new Dictionary<IHero, int>();
		public Dictionary<IEnemy, int> EnemyToAbilitySetMap { get; } = new Dictionary<IEnemy, int>();

		public AbilityService(IEnumerable<IHero> heroes, IEnumerable<IEnemy> enemies)
		{
			List<AbilitySet> abilitySets = new List<AbilitySet>(heroes.Count() + enemies.Count());
			int index = 0;

			foreach (var hero in heroes)
			{
				AbilitySet abilitySet = new AbilitySet(
					hero.Abilities.Select(a => new Ability(a.Name, a.Range, a.Damage, a.Cooldown, a.Projectile)).ToArray()
				);
				abilitySets.Add(abilitySet);
                HeroToAbilitySetMap.Add(hero, index++);
			}

            foreach (var enemy in enemies)
            {
                AbilitySet abilitySet = new AbilitySet(
                    enemy.Abilities.Select(a => new Ability(a.Name, a.Range, a.Damage, a.Cooldown, a.Projectile)).ToArray()
                );
                abilitySets.Add(abilitySet);
                EnemyToAbilitySetMap.Add(enemy, index++);
            }

			_abilitySets = abilitySets.ToArray();
        }

		public AbilitySet GetAbilitySet(int index)
		{
			return _abilitySets[index];
		}
	}
}
