using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.MonoBehaviours;
using UnicornOne.ScriptableObjects;
using UnicornOne.ScriptableObjects.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace UnicornOne.Ecs.Services
{
    internal class LevelService
    {
        public ILevel Level { get; }
        public Vector3[] SpawnPoints { get; private set; }

        public LevelService(Level level)
        {
            Level = level;
        }

        public void Init()
        {
            var spawnPointGameObjects =  GameObject.FindObjectsOfType<SpawnPointScript>();

            SpawnPoints = spawnPointGameObjects.OrderBy(obj => obj.name).Select(obj => obj.transform.position).ToArray();
        }
    }
}
