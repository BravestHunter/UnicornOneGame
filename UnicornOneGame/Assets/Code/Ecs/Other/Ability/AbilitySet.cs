using System;
using System.Collections.Generic;

namespace UnicornOne.Ecs.Other.Ability
{
	public class AbilitySet
	{
        public Ability[] Abilities { get; }

        public AbilitySet(Ability[] abilities)
        {
            Abilities = abilities;
        }
    }
}
