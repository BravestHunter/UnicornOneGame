using System.Collections.Generic;
using System.Linq;
using UnicornOne.Battle.Models;
using UnicornOne.ScriptableObjects;

namespace UnicornOne.Battle.Ecs.Services
{
    internal class AbilityService : IAbilityService
    {
        private readonly AbilitySet[] _abilitySets;
        private readonly Dictionary<Unit, int> _abilityMap;

        public AbilityService(Dictionary<Unit, AbilitySet> abilities)
        {
            _abilitySets = new AbilitySet[abilities.Count];
            _abilityMap = new(abilities.Count);

            int index = 0;
            foreach (var pair in abilities)
            {
                _abilitySets[index] = pair.Value;
                _abilityMap.Add(pair.Key, index);
                index++;
            }
        }

        public AbilitySet GetAbilitySet(int id)
        {
            return _abilitySets[id];
        }

        public int GetAbilitySetIndex(Unit unit)
        {
            return _abilityMap[unit];
        }
    }
}
