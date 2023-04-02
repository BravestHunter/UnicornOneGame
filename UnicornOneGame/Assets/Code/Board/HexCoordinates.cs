using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Board
{
    [Serializable]
    public struct HexCoordinates
    {
        // Axial coords
        public int X;
        public int Y;

        public HexCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public HexCoordinates(Vector2Int vector)
        {
            X = vector.x;
            Y = vector.y;
        }

        public Vector3 ToWorldCoords(float hexOuterRadius, float hexInnerRadius)
        {
            Vector3 position;
            position.x = (X + Y * 0.5f) * (hexInnerRadius * 2f);
            position.y = 0f;
            position.z = Y * (hexOuterRadius * 1.5f);

            return position;
        }

        public static implicit operator Vector2Int(HexCoordinates a) => new Vector2Int(a.X, a.Y);

        public static HexCoordinates operator +(HexCoordinates a, HexCoordinates b)
        {
            return new HexCoordinates(a.X + b.X, a.Y + b.Y);
        }

        public static HexCoordinates operator *(HexCoordinates a, int b)
        {
            return new HexCoordinates(a.X * b, a.Y * b);
        }
    }
}
