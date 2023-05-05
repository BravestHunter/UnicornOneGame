using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Custom/Unit")]
    public class Unit : ScriptableObject
    {
        public GameObject Prefab;
        public int Health;
        public int AttackRange;
        public float Speed;

        public Ability[] Abilities;
    }
}
