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
        private readonly Enemy _enemy;

        public GameObject EnemyPrefab { get { return _enemy.Prefab; } }
        public int MaxHealth { get { return _enemy.Health; } }

        public MobService(Enemy enemy)
        {
            _enemy = enemy;
        }
    }
}
