using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Hero", menuName = "Custom/Hero")]
    internal class Hero : ScriptableObject
    {
        public GameObject Prefab;

        public float MovingSpeed;

        public int AttackDamage;
        public float AttackRange;
        public float AttackRechargeTime;

        public bool IsRanged;
        public Effect AttackEffect;
    }
}
