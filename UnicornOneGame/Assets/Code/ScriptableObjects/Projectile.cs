using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "Custom/Projectile")]
    internal class Projectile : ScriptableObject
    {
        public GameObject Prefab;
        public int Damage;
        public float MoveSpeed;
    }
}
