using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects;
using UnityEngine;
using UnityEngine.AI;

namespace UnicornOne.Ecs.Services
{
    internal class LevelService
    {
        private readonly Level _level;

        public GameObject Prefab { get { return _level.Prefab; } }
        public NavMeshData NavMeshData { get { return _level.NavMeshData; } }

        public LevelService(Level level)
        {
            _level = level;
        }
    }
}
