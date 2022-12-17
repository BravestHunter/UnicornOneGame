using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class LevelInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<LevelService> _levelService;

        public void Init(IEcsSystems systems)
        {
            GameObject.Instantiate(_levelService.Value.Prefab);
        }
    }
}
