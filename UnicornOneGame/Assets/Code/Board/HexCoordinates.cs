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

        public Vector3 ToWorldCoords(float hexOuterRadius, float hexInnerRadius)
        {
            Vector3 position;
            position.x = (X + Y * 0.5f) * (hexInnerRadius * 2f);
            position.y = 0f;
            position.z = Y * (hexOuterRadius * 1.5f);

            return position;
        }
    }
}
