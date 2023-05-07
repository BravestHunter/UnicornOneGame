using UnicornOne.Battle.Models;
using UnicornOne.ScriptableObjects;

namespace UnicornOne.Battle.Ecs.Services
{
    internal interface IAbilityService : IService
    {
        public AbilitySet GetAbilitySet(int id);
        public int GetAbilitySetIndex(Unit unit);
    }
}
