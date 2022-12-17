﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level", menuName = "Custom/Level")]
    internal class Level : ScriptableObject
    {
        public GameObject Prefab;
        public NavMeshData NavMeshData;
    }
}
