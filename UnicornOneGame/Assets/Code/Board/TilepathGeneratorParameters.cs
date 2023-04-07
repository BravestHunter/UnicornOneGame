using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Board
{
    [Serializable]
    public struct TilepathGeneratorParameters
    {
        public Tile StartTile;
        public Tile FinishTile;
        public Tile RoadTile;
        public int Length;
    }
}
