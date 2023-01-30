using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects.Data;
using UnityEngine;
using UnityEngine.AI;

namespace UnicornOne.ScriptableObjects.Interfaces
{
    internal interface ILevel
    {
        PrefabInfo PrefabInfo { get; }
        NavMeshData NavMeshData { get; }
        Material Skybox { get; }
        LevelScript Script { get; }
        public Vector3[] EnemySpawnPositions { get; }
    }
}
