using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Assets.Code.ScriptableObjects.Data;
using UnicornOne.ScriptableObjects.Data;
using UnicornOne.ScriptableObjects.Interfaces;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Custom/Enemy")]
    internal class Enemy : ScriptableObject, IEnemy
    {
        public PrefabInfo Prefab;
        public MoveInfo Move;
        public HealthInfo Health;
        public AttackInfo Attack;

        public PrefabInfo PrefabInfo => Prefab;
        public MoveInfo MoveInfo => Move;
        public HealthInfo HealthInfo => Health;
        public AttackInfo AttackInfo => Attack;
    }
}
