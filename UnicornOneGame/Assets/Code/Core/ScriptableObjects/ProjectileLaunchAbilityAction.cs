using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ProjectileLaunchAbilityAction", menuName = "Custom/ProjectileLaunchAbilityAction")]
    public class ProjectileLaunchAbilityAction : BaseAbilityAction
    {
        public Projectile Projectile;
        public int Damage;
        public float Speed;
    }
}
