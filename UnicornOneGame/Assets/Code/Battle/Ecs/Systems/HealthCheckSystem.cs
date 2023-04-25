using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class HealthCheckSystem : IEcsRunSystem
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

            var healthComponentPool = world.GetPool<HealthComponent>();
            var destroyFlagPool = world.GetPool<DestroyFlag>();

            foreach (var entity in _filter)
            {
                var healthComponent = healthComponentPool.Get(entity);

                if (healthComponent.Current <= 0)
                {
                    destroyFlagPool.Add(entity);
                }
            }
        }
    }
}
