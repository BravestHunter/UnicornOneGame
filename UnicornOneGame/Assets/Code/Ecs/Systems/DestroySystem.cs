using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Components.Refs;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class DestroySystem : IEcsRunSystem
    {
        private EcsFilter _destroyRequestFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_destroyRequestFilter == null)
            {
                _destroyRequestFilter = world
                    .Filter<DestroyRequest>()
                    .End();
            }

            var gameObjectRefPool = world.GetPool<GameObjectRefComponent>();

            foreach (var entity in _destroyRequestFilter)
            {
                if (gameObjectRefPool.Has(entity))
                {
                    ref var gameObjectRefComponent = ref gameObjectRefPool.Get(entity);
                    GameObject.Destroy(gameObjectRefComponent.GameObject);
                }

                world.DelEntity(entity);
            }
        }
    }
}
