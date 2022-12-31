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
        public float AttackDelay => _hero.AttackDelay;

        public HeroInfo(Hero hero)
        {
            _hero = hero;
        }
    }
}
