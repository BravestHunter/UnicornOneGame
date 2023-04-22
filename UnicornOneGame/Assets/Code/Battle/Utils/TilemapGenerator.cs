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

                    var tile = new Tile();
                    tile.IsWalkable = Random.value >= 0.15f;
                    tilemap.Tiles[HexCoords.FromCube(q, r, s)] = tile;
                }
            }

            tilemap.Tiles[HexCoords.Center].IsWalkable = true;

            return tilemap;
        }
    }
}
