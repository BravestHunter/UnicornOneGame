using System.Collections.Generic;
using UnicornOne.Core.Utils;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Utils
{
    public static class TilemapGenerator
    {
        public static Tilemap Generate(int radius, float unwalkableDencity)
        {
            Debug.Assert(radius >= 0);
            Debug.Assert(unwalkableDencity >= 0 && unwalkableDencity < 1);

            List<TileEntry> tileEntries = new List<TileEntry>();
            for (int q = -radius; q <= radius; q++)
            {
                int rFrom = System.Math.Max(-radius, -q - radius);
                int rTo = System.Math.Min(radius, -q + radius);
                for (int r = rFrom; r <= rTo; r++)
                {
                    TileEntry tileEntry = new TileEntry();

                    int s = -q - r;
                    tileEntry.Position = HexCoords.FromCube(q, r, s);

                    tileEntry.IsWalkable = Random.value >= unwalkableDencity;

                    tileEntries.Add(tileEntry);
                }
            }

            Tilemap tilemap = new Tilemap() { Tiles = tileEntries.ToArray() };

            return tilemap;
        }
    }
}
