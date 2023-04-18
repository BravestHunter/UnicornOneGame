using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Models;
using UnicornOne.Core.Utils;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Services
{
    internal class TilemapService : ITilemapService
    {
        public GameObject TilePrefab { get; private set; }
        public Tilemap Tilemap { get; private set; }

        public TilemapService(GameObject tilePrefab, int radius) 
        {
            TilePrefab = tilePrefab;

            Tilemap = new Tilemap();

            for (int q = -radius; q <= radius; q++)
            {
                int rFrom = Math.Max(-radius, -q - radius);
                int rTo = Math.Min(radius, -q + radius);
                for (int r = rFrom; r <= rTo; r++)
                {
                    int s = -q - r;
                    Tilemap[HexCoords.FromCube(q, r, s)] = new Tile();
                }
            }
        }
    }
}
