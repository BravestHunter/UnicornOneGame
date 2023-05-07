using System.Collections;
using System.Collections.Generic;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Battle.Models
{
    public class AbilitySet
    {
        public Ability[] Abilities { get; private set; }

        public AbilitySet(Ability[] abilities)
        {
            Abilities = abilities;
        }
    }
}
