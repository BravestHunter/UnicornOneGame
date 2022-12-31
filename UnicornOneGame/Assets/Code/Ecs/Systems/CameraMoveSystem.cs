using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components.Flags;
using UnicornOne.Ecs.Components.Refs;
using UnicornOne.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class CameraMoveSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<CameraService> _cameraService;

        private EcsFilter _heroFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_heroFilter == null)
            {
                _heroFilter = world
                    .Filter<HeroFlag>()
                    .Inc<GameObjectRefComponent>()
                    .End();
            }

            var gameObjectRefPool = world.GetPool<GameObjectRefComponent>();

            List<Vector3> heroPositions = new List<Vector3>();
            foreach (var entity in _heroFilter)
            {
                var gameObjectRefComponent = gameObjectRefPool.Get(entity);
                heroPositions.Add(gameObjectRefComponent.GameObject.transform.position);
            }

            if (heroPositions.Count > 0)
            {
                var bounds = GetBounds(heroPositions);
                _cameraService.Value.MoveToFitBounds(bounds);
            }
        }

        private Bounds GetBounds(IEnumerable<Vector3> positions)
        {
            Bounds bounds = new Bounds();

            foreach (Vector3 position in positions)
            {
                bounds.Encapsulate(position);
            }

            return bounds;
        }
    }
}
