using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Core.Geometry
{
    public readonly struct Segment
    {
        public readonly Vector3 A;
        public readonly Vector3 B;

        public Segment(Vector3 a, Vector3 b)
        {
            A = a;
            B = b;
        }
    }
}
