using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Components.Refs;

namespace UnicornOne.Ecs.Systems
{
    internal class NavigationSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<NavigationAgentRefComponent>()
                    .Inc<NavigationComponent>()
                    .End();
            }

            var navigationAgentRefPool = world.GetPool<NavigationAgentRefComponent>();
            var navigationPool = world.GetPool<NavigationComponent>();

            foreach (var entity in _filter)
            {
                ref var navigationAgentRefComponent = ref navigationAgentRefPool.Get(entity);
                ref var navigationComponent = ref navigationPool.Get(entity);

                navigationAgentRefComponent.Agent.destination = navigationComponent.DestionationPosition;
                navigationAgentRefComponent.Agent.speed = navigationComponent.MovementSpeed;
            }
        }
    }
}
