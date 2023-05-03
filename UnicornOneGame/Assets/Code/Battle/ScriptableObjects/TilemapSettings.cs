using System.Collections;
using System.Collections.Generic;
using UnicornOne.Core.Utils;
using UnityEngine;

namespace UnicornOne.Battle.ScriptableObjects
{
    [CreateAssetMenu(fileName = "TilemapSettings", menuName = "Custom/Battle/TilemapSettings")]
    internal class TilemapSettings : ScriptableObject
    {
        public float HexOuterRadius = 1.0f;

        public HexParams HexParams => HexParams.FromOuterRadius(HexOuterRadius);
    }
}
