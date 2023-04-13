using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class LifetimeSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<LifetimeComponent>()
                    .End();
            }

            var lifetimePool = world.GetPool<LifetimeComponent>();
            var destroyRequestPool = world.GetPool<DestroyRequest>();

            foreach (var entity in _filter)
            {
                var lifetimeComponent = lifetimePool.Get(entity);

                if (Time.timeSinceLevelLoad >= lifetimeComponent.CreationTime + lifetimeComponent.LifeDuration)
                {
                    destroyRequestPool.Add(entity);
                }
            }
        }
    }
}
