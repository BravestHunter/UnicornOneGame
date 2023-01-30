using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Services;
using UnityEngine;
using UnityEngine.AI;

namespace UnicornOne.Ecs.Systems
{
    internal class LevelInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<LevelService> _levelService;

        public void Init(IEcsSystems systems)
        {
            var levelGameObject = GameObject.Instantiate(_levelService.Value.Level.PrefabInfo.Prefab);

            NavMesh.AddNavMeshData(_levelService.Value.Level.NavMeshData);

            RenderSettings.skybox = _levelService.Value.Level.Skybox;

            _levelService.Value.Init();
        }
    }
}
