using System;

namespace UnicornOne.Board
{
	[Serializable]
	public struct TilemapTileParameters
	{
        public float HexOuterRadius;
        public float HexInnerRadius => HexOuterRadius * 0.866025404f; // * sqrRoot(3) / 2
        public float Height;
        public float BorderSize;
    }
}
