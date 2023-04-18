using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicornOne.Core.Utils
{
    /// <summary>
    /// Represents size parameters of point-top-oriented hexagon.
    /// </summary>
    public struct HexParams
    {
        private const float Sqr3 = 1.7320508f;

        public readonly float Width;
        public readonly float Height;
        public readonly float OuterRadius;
        public readonly float InnerRadius;

        private HexParams(float outerRadius)
        {
            if (outerRadius <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(outerRadius));
            }

            Width = outerRadius * Sqr3;
            Height = outerRadius * 2;
            OuterRadius = outerRadius;
            InnerRadius = Width / 2;
        }

        public static HexParams FromOuterRadius(float outerRadius)
        {
            return new HexParams(outerRadius);
        }

        public static HexParams FromInnerRadius(float innerRadius)
        {
            return new HexParams(innerRadius * 2 / Sqr3);
        }
    }
}
