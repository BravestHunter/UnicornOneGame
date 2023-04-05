using System;

namespace UnicornOne.Board
{
    [Serializable]
    public struct TilemapFillParameters
    {
        public bool Fill;
        public HexCoordinates Center;
        public int Radius;
        public Tile Tile;
    }
}

