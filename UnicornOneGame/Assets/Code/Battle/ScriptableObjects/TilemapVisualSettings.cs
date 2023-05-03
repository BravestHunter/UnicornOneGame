using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Battle.ScriptableObjects
{
    [CreateAssetMenu(fileName = "TilemapVisualSettings", menuName = "Custom/Battle/TilemapVisualSettings")]
    internal class TilemapVisualSettings : ScriptableObject
    {
        public GameObject TilePrefab;
        public Material TileWalkableMaterial;
        public Material TileUnwalkableMaterial;
    }
}
