using UnicornOne.Battle.Models;
using UnicornOne.Core.Utils;

namespace UnicornOne.Battle.Ecs.Services
{
    internal interface ITilemapService : IService
    {
        public Tilemap Tilemap { get; }
        public HexParams HexParams { get; }

        public HexCoords GetRandomAvailablePosition();
    }
}
