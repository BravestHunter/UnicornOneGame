using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicornOne.Ecs.Components
{
    internal struct AtackParametersComponent
    {
        public int Damage;
        public float Range;
        public float AttackRechargeTime;
    }
}
