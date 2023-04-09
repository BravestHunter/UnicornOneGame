using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;

namespace UnicornOne.Ecs.Systems
{
    internal class DeathSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<HealthComponent>()
                    .End();
            }

            var healthPool = world.GetPool<HealthComponent>();
            var destroyRequestPool = world.GetPool<DestroyRequest>();

            foreach (var entity in _filter)
            {
                ref var healthComponent = ref healthPool.Get(entity);

                if (healthComponent.CurrentHealth <= 0)
                {
                    destroyRequestPool.Add(entity);
                }
            }
        }
    }
}
