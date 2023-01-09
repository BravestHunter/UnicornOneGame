using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects.Data;
using UnicornOne.ScriptableObjects.Interfaces;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Effect", menuName = "Custom/Effect")]
    internal class Effect : ScriptableObject, IEffect
    {
        public PrefabInfo Prefab;

        public PrefabInfo PrefabInfo => Prefab;
    }
}
