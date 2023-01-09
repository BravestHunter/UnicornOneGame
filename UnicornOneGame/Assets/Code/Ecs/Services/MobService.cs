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
        public IEnemy Enemy { get; }

        public MobService(Enemy enemy)
        {
            Enemy = enemy;
        }
    }
}
