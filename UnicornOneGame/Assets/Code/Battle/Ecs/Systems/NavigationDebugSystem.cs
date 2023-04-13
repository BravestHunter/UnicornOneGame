using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class NavigationDebugSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<NavMeshAgentUnityRefComponent>()
                    .End();
            }

            var navMeshAgentUnityRefComponentPool = world.GetPool<NavMeshAgentUnityRefComponent>();

            foreach (var entity in _filter)
            {
                var navMeshAgentUnityRefComponent = navMeshAgentUnityRefComponentPool.Get(entity);

                Debug.DrawLine(navMeshAgentUnityRefComponent.NavMeshAgent.gameObject.transform.position, navMeshAgentUnityRefComponent.NavMeshAgent.destination, Color.blue);
            }
        }
    }
}
