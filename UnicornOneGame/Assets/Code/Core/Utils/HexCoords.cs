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
    public readonly struct HexCoords
    {
        public readonly int Q;
        public readonly int R;

        private HexCoords(int q, int r)
        {
            Q = q;
            R = r;
        }

        public override bool Equals(object obj)
        {
            if (obj is HexCoords hexCoords)
            {
                return hexCoords == this;
            }

            return false;
        }

        public static bool operator==(HexCoords a, HexCoords b)
        {
            return a.Q == b.Q && a.R == b.R;
        }

        public static bool operator !=(HexCoords a, HexCoords b)
        {
            return !(a == b);
        }

        public static HexCoords FromAxial(int q, int r)
        {
            return new HexCoords(q, r);
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

        public int DistanceTo(in HexCoords hexCoords)
        {
            return Distance(this, hexCoords);
        }

        public static int Distance(in HexCoords a, in HexCoords b)
        {
            return (Math.Abs(a.Q - b.Q) + Math.Abs(a.Q + a.R - b.Q - b.R) + Math.Abs(a.R - b.R)) / 2;
        }
    }
}
