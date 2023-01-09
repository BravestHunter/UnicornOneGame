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

        public PrefabInfo PrefabInfo => Prefab;
        public NavMeshData NavMeshData => NavMesh;
    }
}
