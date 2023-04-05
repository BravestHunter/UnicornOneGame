using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Board
{
    [CreateAssetMenu(fileName = "TilemapSettings", menuName = "Custom/Board/TilemapSettings")]
    public class TilemapSettings : ScriptableObject
    {
        public TilemapTileParameters TileParameters;
        public TilemapFillParameters FillParameters;
    }
}
