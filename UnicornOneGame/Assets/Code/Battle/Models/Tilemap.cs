using System.Collections;
using System.Collections.Generic;
using UnicornOne.Core.Utils;

namespace UnicornOne.Battle.Models
{
    internal class Tilemap : IEnumerable<KeyValuePair<HexCoords, Tile>>
    {
        private readonly Dictionary<HexCoords, Tile> _tiles;

        public Tile? this[HexCoords coords]
        {
            get
            {
                Tile tile;
                if (_tiles.TryGetValue(coords, out tile))
                {
                    return tile;
                }

                return null;
            }
            set
            {
                if (!value.HasValue)
                {
                    return;
                }

                _tiles[coords] = value.Value;
            }
        }

        public Tilemap()
        {
            _tiles = new Dictionary<HexCoords, Tile>();
        }

        public IEnumerator<KeyValuePair<HexCoords, Tile>> GetEnumerator()
        {
            return _tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
