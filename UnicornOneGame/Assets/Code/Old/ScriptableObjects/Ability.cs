using System;
using UnityEngine;
using UnicornOne.ScriptableObjects.Interfaces;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Custom/Old/Ability")]
    public class AbilityOld : ScriptableObject, IAbility
    {
		public string Name;
		public float Range;
		public int Damage;
		public float Cooldown;

        public Projectile Projectile;
        public Effect Effect;

        string IAbility.Name => Name;
        float IAbility.Range => Range;
        int IAbility.Damage => Damage;
        float IAbility.Cooldown => Cooldown;
        Projectile IAbility.Projectile => Projectile;
        IEffect IAbility.Effect => Effect;
    }
}
