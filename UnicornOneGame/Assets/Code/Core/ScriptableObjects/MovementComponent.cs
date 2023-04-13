using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Movement", menuName = "Custom/UnitComponents/Movement")]
    public class MovementComponent : UnitComponent
    {
        public float Speed;
        public float AngularSpeed;
        public float Acceleration;
    }
}
