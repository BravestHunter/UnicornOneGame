using System.Collections;
using System.Collections.Generic;
using UnicornOne.Battle.Models;
using UnicornOne.Core.Utils;
using UnityEngine;

namespace UnicornOne.Battle.Utils
{
    internal static class TilemapGenerator
    {
        public static Tilemap Generate(int radius)
        {
            Tilemap tilemap = new Tilemap();

            for (int q = -radius; q <= radius; q++)
            {
                int rFrom = System.Math.Max(-radius, -q - radius);
                int rTo = System.Math.Min(radius, -q + radius);
                for (int r = rFrom; r <= rTo; r++)
                {
                    int s = -q - r;
                    HexCoords coords = HexCoords.FromCube(q, r, s);

                    bool isWalkable = Random.value >= 0.15f;
                    var tile = new Tile(isWalkable);
                    tilemap.Tiles[coords] = tile;
                }
            }

            return tilemap;
        }
    }
}
