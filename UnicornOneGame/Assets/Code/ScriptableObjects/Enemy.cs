using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Custom/Enemy")]
    internal class Enemy : ScriptableObject
    {
        public GameObject Prefab;

        public int Health;
    }
}
