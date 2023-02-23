using System;

namespace UnicornOne.ScriptableObjects.Interfaces
{
    public interface IAbility
    {
        public string Name { get; }
        public float Range { get; }
        public int Damage { get; }
        public float Cooldown { get; }
    }
}
