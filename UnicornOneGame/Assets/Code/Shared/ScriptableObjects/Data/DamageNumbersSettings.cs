using System;
using TMPro;
using UnityEngine;

namespace UnicornOne.ScriptableObjects.Data
{
    [Serializable]
    public struct DamageNumbersSettings
    {
        public GameObject Prefab;
        public TMP_FontAsset Font;
        public float FontSize;
        public Color Color;
        public float Lifetime;
        [Tooltip("X is left-right, Y is up-down, Z is forward-backward to camera")]
        public Vector3 SpawnPointOffset;
    }
}
