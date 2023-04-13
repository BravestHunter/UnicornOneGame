using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
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
                    .Filter<NavMeshAgentUnityRefComponent>()
                    .End();
            }

            var navMeshAgentUnityRefComponentPool = world.GetPool<NavMeshAgentUnityRefComponent>();

            foreach (var entity in _filter)
            {
                var navMeshAgentUnityRefComponent = navMeshAgentUnityRefComponentPool.Get(entity);

                if ((navMeshAgentUnityRefComponent.NavMeshAgent.destination - navMeshAgentUnityRefComponent.NavMeshAgent.gameObject.transform.position).sqrMagnitude < 0.1f)
                {
                    Vector2 random = Random.insideUnitCircle * 10.0f;

                    navMeshAgentUnityRefComponent.NavMeshAgent.destination = new Vector3(random.x, 0.0f, random.y);
                }
            }
        }
    }
}
