using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.Core.Utils
{
    /// <summary>
    /// Represents universal hexagonal grid coordinates.
    /// Internally implemented as axial coordiantes.
    /// </summary>
    public struct HexCoords
    {
        public readonly int Q;
        public readonly int R;

        private HexCoords(int q, int r)
        {
            Q = q;
            R = r;
        }

        public static HexCoords FromAxial(int r, int s)
        {
            return new HexCoords(r, s);
        }

        public static HexCoords FromCube(int q, int r, int s = 0)
        {
            return new HexCoords(q, r);
        }

        public Vector2 ToWorldCoords(in HexParams hexParams)
        {
            Vector2 position;
            position.x = (Q + R * 0.5f) * (hexParams.InnerRadius * 2f);
            position.y = R * (hexParams.OuterRadius * 1.5f);

            return position;
        }

        public Vector3 ToWorldCoordsXZ(in HexParams hexParams)
        {
            Vector2 coords = ToWorldCoords(hexParams);

            return new Vector3(coords.x, 0.0f, coords.y);
        }
    }
}
