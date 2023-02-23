using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects.Data;
using UnicornOne.ScriptableObjects.Interfaces;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "Custom/Projectile")]
    public class Projectile : ScriptableObject, IProjectile
    {
        public PrefabInfo Prefab;
        public MoveInfo Move;

        public PrefabInfo PrefabInfo => Prefab;
        public MoveInfo MoveInfo => Move;
    }
}
