using System;
using UnicornOne.Ecs.Other.Ability;

namespace UnicornOne.Ecs.Services
{
	public class AbilityService
	{
		private AbilitySet[] _abilitySets;

		public AbilityService()
		{
			AbilitySet heroAbilitySet = new(new Ability[]
			{
				new Ability("Attack", 2.5f, 15, 1),
				new Ability("CutAttack", 2.5f, 30, 5)
			});

            AbilitySet enemyAbilitySet = new(new Ability[]
            {
                new Ability("Attack", 2.0f, 10, 2)
            });

			_abilitySets = new AbilitySet[] { heroAbilitySet, enemyAbilitySet };
        }

		public AbilitySet GetAbilitySet(int index)
		{
			return _abilitySets[index];
		}
	}
}
