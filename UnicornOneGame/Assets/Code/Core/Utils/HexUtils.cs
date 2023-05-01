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

        public static IEnumerable<HexCoords> InRange(HexCoords center, int range)
        {
            if (range <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(range));
            }

            for (int q = - range; q <= range; q++)
            {
                int from = Math.Max(-range, -q - range);
                int to = Math.Min(range, -q + range);

                for (int r = from; r <=to; r++)
                {
                    int s = -q - r;

                    yield return center + HexCoords.FromCube(q, r, s);
                }
            }
        }
    }
}
