using UnicornOne.ScriptableObjects;

namespace UnicornOne.Battle.Ecs.Services
{
    internal interface IAbilityService : IService
    {
        public Ability GetAbility(int id);
    }
}
