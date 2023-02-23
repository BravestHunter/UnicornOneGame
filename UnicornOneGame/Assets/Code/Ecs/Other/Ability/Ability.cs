﻿using System;
using UnicornOne.ScriptableObjects.Interfaces;

namespace UnicornOne.Ecs.Other.Ability
{
    public class Ability
    {
        public string Name { get; }
        public float Range { get; }
        public int Damage { get; }
        public float Cooldown { get; }

        public IProjectile Projectile { get; }

        public Ability(string name, float range, int damage, float cooldown, IProjectile projectile)
        {
            Name = name;
            Range = range;
            Damage = damage;
            Cooldown = cooldown;
            Projectile = projectile;
        }
    }
}
