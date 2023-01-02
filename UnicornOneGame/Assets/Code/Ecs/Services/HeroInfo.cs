using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Ecs.Services
{
    internal class HeroInfo
    {
        private Hero _hero;

        public GameObject Prefab => _hero.Prefab;
        public float MovingSpeed => _hero.MovingSpeed;
        public int AttackDamage => _hero.AttackDamage;
        public float AttackRange => _hero.AttackRange;
        public float AttackRechargeTime => _hero.AttackRechargeTime;
        public bool IsRanged => _hero.IsRanged;
        public bool HasAttackEffect => _hero.AttackEffect != null;

        public HeroInfo(Hero hero)
        {
            _hero = hero;
        }
    }
}
