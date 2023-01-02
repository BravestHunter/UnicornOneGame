using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Effect", menuName = "Custom/Effect")]
    internal class Effect : ScriptableObject
    {
        public GameObject Prefab;
    }
}
