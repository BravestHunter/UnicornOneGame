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
    [CreateAssetMenu(fileName = "Hero", menuName = "Custom/Old/Hero")]
    public class Hero : ScriptableObject, IHero
    {
        public PrefabInfo Prefab;
        public MoveInfo Move;
        public HealthInfo Health;
        public Ability[] Abilities;

        public PrefabInfo PrefabInfo => Prefab;
        public MoveInfo MoveInfo => Move;
        public HealthInfo HealthInfo => Health;
        IEnumerable<IAbility> IHero.Abilities => Abilities;
    }
}
