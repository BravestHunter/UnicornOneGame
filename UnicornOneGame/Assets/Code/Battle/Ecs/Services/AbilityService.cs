using UnicornOne.ScriptableObjects;

namespace UnicornOne.Battle.Ecs.Services
{
    internal class AbilityService : IAbilityService
    {
        private readonly Ability[] _abilities;

        public AbilityService(Ability[] abilities)
        {
            _abilities = abilities;
        }

        public Ability GetAbility(int id)
        {
            return _abilities[id];
        }
    }
}
