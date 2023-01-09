using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects;

namespace UnicornOne.Assets.Code.ScriptableObjects.Data
{
    [Serializable]
    internal struct AttackInfo
    {
        public int Damage;
        public float Range;
        public float RechargeTime;

        public bool IsRanged;
        public Effect AttackEffect;
    }
}
