using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "Custom/Projectile")]
    public class Projectile : ScriptableObject
    {
        public GameObject Prefab;
    }
}
