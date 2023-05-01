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
        public static HexCoords Center => FromAxial(0, 0);

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

        public static HexCoords operator+(HexCoords a, HexCoords b)
        {
            return HexCoords.FromAxial(a.Q + b.Q, a.R + b.R);
        }

        public static HexCoords FromAxial(int q, int r)
        {
            return new HexCoords(q, r);
        }

        public static HexCoords FromCube(int q, int r, int s = 0)
        {
            return new HexCoords(q, r);
        }

        public static HexCoords FromWorldCoords(in Vector2 position, in HexParams hexParams)
        {
            // Calculate float coords
            var qF = (MathF.Sqrt(3) / 3.0f * position.x - 1.0f / 3.0f * position.y) / hexParams.OuterRadius;
            var rF = (2.0f / 3.0f * position.y) / hexParams.OuterRadius;
            float sF = -qF - rF;

            int q = (int)MathF.Round(qF);
            int r = (int)MathF.Round(rF);
            int s = (int)MathF.Round(sF);

            var qDiff = MathF.Abs(q - qF);
            var rDiff = MathF.Abs(r - rF);
            var sDiff = MathF.Abs(s - sF);

            if (qDiff > rDiff && qDiff > sDiff)
            {
                q = -r - s;
            }
            else if (rDiff > sDiff)
            {
                r = -q - s;
            }
            else
            {
                s = -q - r;
            }

            return FromCube(q, r, s);
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
