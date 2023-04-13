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
    [CreateAssetMenu(fileName = "Effect", menuName = "Custom/Old/Effect")]
    public class Effect : ScriptableObject, IEffect
    {
        public PrefabInfo Prefab;

        PrefabInfo IEffect.Prefab => Prefab;
    }
}
