using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Components
{
    internal struct AttackParamsComponent
    {
        public int Damage;
        public float Cooldown;
        public int Range;
    }
}
