using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Ecs.Services
{
    internal class EffectService
    {
        private readonly Effect _effect;

        public GameObject Prefab => _effect.Prefab;

        public EffectService(Effect effect)
        {
            _effect = effect;
        }
    }
}
