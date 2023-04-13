using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Health", menuName = "Custom/UnitComponents/Health")]
    public class HealthComponent : UnitComponent
    {
        public int Health;
    }
}
