using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Models;
using UnicornOne.Battle.MonoBehaviours;
using UnicornOne.Battle.ScriptableObjects;
using UnicornOne.Core.Utils;
using UnityEngine;
using UnityEngine.XR;

namespace UnicornOne.Battle.Ecs.Services
{
    internal class TilemapService : ITilemapService
    {
        public Tilemap Tilemap { get; }
        public HexParams HexParams { get; }

        public TilemapService(Tilemap tilemap, TilemapSettings setting) 
        {
            Tilemap = tilemap;
            HexParams = setting.HexParams;
        }

        public HexCoords GetRandomAvailablePosition()
        {
            KeyValuePair<HexCoords, Tile> tileEntry;
            do
            {
                tileEntry = Tilemap.Tiles.ElementAt(Random.Range(0, Tilemap.Tiles.Count));
            }
            while (!tileEntry.Value.IsAvailable);

            return tileEntry.Key;
        }
    }
}
