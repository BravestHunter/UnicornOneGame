﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Custom/Unit")]
    public class Unit : ScriptableObject
    {
        public GameObject Prefab;
        public int Health;
        public int Damage;
        public float Speed;
    }
}
