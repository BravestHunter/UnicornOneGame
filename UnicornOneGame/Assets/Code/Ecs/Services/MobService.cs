using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Ecs.Services
{
    internal class MobService
    {
        private readonly Mob _enemy;

        public GameObject EnemyPrefab { get { return _enemy.Prefab; } }

        public MobService(Mob enemy)
        {
            _enemy = enemy;
        }
    }
}
