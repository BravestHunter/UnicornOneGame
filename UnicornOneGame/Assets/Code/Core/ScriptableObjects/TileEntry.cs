using System;
using UnicornOne.Core.Utils;

namespace UnicornOne.ScriptableObjects
{
    [Serializable]
    public struct TileEntry
    {
        public HexCoords Position;
        public bool IsWalkable;
    }
}
