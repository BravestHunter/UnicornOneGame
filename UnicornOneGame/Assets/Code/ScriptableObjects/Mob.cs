using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Mob", menuName = "Custom/Mob")]
    internal class Mob : ScriptableObject
    {
        public GameObject Prefab;

        public int Health;
    }
}
