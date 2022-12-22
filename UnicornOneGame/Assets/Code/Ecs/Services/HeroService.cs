using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Ecs.Services
{
    internal class HeroService
    {
        private readonly Hero _hero;

        public GameObject Prefab { get { return _hero.Prefab; } }
        public float MovingSpeed { get { return _hero.MovingSpeed; } }
        public int AttackDamage { get { return _hero.AttackDamage; } }
        public float AttackRange { get { return _hero.AttackRange; } }
        public float AttackDelay { get { return _hero.AttackDelay; } }

        public HeroService(Hero hero)
        {
            _hero = hero;
        }
    }
}
