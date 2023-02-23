using System;
using UnityEngine;
using UnicornOne.ScriptableObjects.Interfaces;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Custom/Ability")]
    public class Ability : ScriptableObject, IAbility
    {
		public string Name;
		public float Range;
		public int Damage;
		public float Cooldown;

        public Projectile Projectile;

        string IAbility.Name => Name;
        float IAbility.Range => Range;
        int IAbility.Damage => Damage;
        float IAbility.Cooldown => Cooldown;
        Projectile IAbility.Projectile => Projectile;
    }
}
