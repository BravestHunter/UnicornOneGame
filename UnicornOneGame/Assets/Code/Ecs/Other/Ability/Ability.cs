using System;

namespace UnicornOne.Ecs.Other.Ability
{
    public class Ability
    {
        public string Name { get; }
        public float Range { get; }
        public int Damage { get; }
        public float Cooldown { get; }

        public Ability(string name, float range, int damage, float cooldown)
        {
            Name = name;
            Range = range;
            Damage = damage;
            Cooldown = cooldown;
        }
    }
}
