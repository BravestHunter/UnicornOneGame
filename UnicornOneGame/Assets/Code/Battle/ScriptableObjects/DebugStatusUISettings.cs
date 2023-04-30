using UnityEngine;

namespace UnicornOne.Battle.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DebugStatusUISettings", menuName = "Custom/Battle/Debug/DebugStatusUISettings")]
    public class DebugStatusUISettings : ScriptableObject
    {
        public GameObject Prefab;
        public float FontSize;
        public Color AllyColor;
        public Color EnemyColor;
        public Vector3 OffsetFromObject;
        public float DistanceFromCamera;
        public bool ShowHpInfo;
        public bool ShowAiInfo;
    }
}
