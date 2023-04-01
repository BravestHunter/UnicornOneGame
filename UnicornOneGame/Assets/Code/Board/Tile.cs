using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Board
{
    [CreateAssetMenu(fileName = "Tile", menuName = "Custom/Board/Tile")]
    public class Tile : ScriptableObject
    {
        public Color Color;

        // All tile data including tile script
    }
}
