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
        private readonly List<HeroInfo> _heroes;

        public IEnumerable<HeroInfo> Heroes => _heroes;

        public HeroService(IEnumerable<Hero> heroes)
        {
            _heroes = heroes.Select(hero => new HeroInfo(hero)).ToList();
        }
    }
}
