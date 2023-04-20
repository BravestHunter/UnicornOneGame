using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicornOne.Core.Utils
{
    public static class HexUtils
    {
        public static IEnumerable<HexCoords> GetNeighbors(HexCoords hexCoords)
        {
            yield return HexCoords.FromAxial(hexCoords.Q + 1, hexCoords.R);
            yield return HexCoords.FromAxial(hexCoords.Q, hexCoords.R + 1);
            yield return HexCoords.FromAxial(hexCoords.Q - 1, hexCoords.R + 1);
            yield return HexCoords.FromAxial(hexCoords.Q - 1, hexCoords.R);
            yield return HexCoords.FromAxial(hexCoords.Q, hexCoords.R - 1);
            yield return HexCoords.FromAxial(hexCoords.Q + 1, hexCoords.R - 1);
        }
    }
}
