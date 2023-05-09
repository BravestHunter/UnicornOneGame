using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Tilemap", menuName = "Custom/Tilemap")]
    public class Tilemap : ScriptableObject
    {
        public TileEntry[] Tiles;
    }
}
