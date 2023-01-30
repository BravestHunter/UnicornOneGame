using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects.Data;
using UnicornOne.ScriptableObjects.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level", menuName = "Custom/Level")]
    internal class Level : ScriptableObject, ILevel
    {
        public PrefabInfo Prefab;
        public NavMeshData NavMesh;
        public Material Skybox;
        public LevelScript Script;
        public Vector3[] EnemySpawnPositions;

        public PrefabInfo PrefabInfo => Prefab;
        public NavMeshData NavMeshData => NavMesh;
        Material ILevel.Skybox => Skybox;
        LevelScript ILevel.Script => Script;
        Vector3[] ILevel.EnemySpawnPositions => EnemySpawnPositions;
    }
}
