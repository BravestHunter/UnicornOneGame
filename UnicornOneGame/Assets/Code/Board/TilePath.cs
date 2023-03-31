using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Board
{
    [CreateAssetMenu(fileName = "TilePath", menuName = "Custom/Board/TilePath")]
    public class TilePath : ScriptableObject
    {
        [Serializable]
        public struct TileEntry
        {
            public Vector3Int _position;
            public Tile _tile;
        }

        public TileEntry[] Path;
    }
}
