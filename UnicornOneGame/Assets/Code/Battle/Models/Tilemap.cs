using System.Collections;
using System.Collections.Generic;
using UnicornOne.Core.Utils;

namespace UnicornOne.Battle.Models
{
    internal class Tilemap : IEnumerable<KeyValuePair<HexCoords, Tile>>
    {
        public Dictionary<HexCoords, Tile> Tiles { get; private set; }

        public Tilemap(UnicornOne.ScriptableObjects.Tilemap tilemap)
        {
            Tiles = new Dictionary<HexCoords, Tile>(tilemap.Tiles.Length);

            foreach (var tileEntry in tilemap.Tiles)
            {
                Tiles.Add(tileEntry.Position, new Tile(tileEntry.IsWalkable));
            }
        }

        public IEnumerator<KeyValuePair<HexCoords, Tile>> GetEnumerator()
        {
            return Tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
