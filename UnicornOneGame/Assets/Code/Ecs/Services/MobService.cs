using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects;
using UnicornOne.ScriptableObjects.Interfaces;
using UnityEngine;

namespace UnicornOne.Ecs.Services
{
    internal class MobService
    {
        private readonly Enemy _enemy;

        public IEnemy Enemy => _enemy;

        public MobService(Enemy enemy)
        {
            _enemy = enemy;
        }
    }
}
