using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class DamageSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<DamageComponent>()
                    .Inc<TargetEntityComponent>()
                    .End();
            }

            var damageComponentPool = world.GetPool<DamageComponent>();
            var targetEntityComponentPool = world.GetPool<TargetEntityComponent>();
            var healthComponentPool = world.GetPool<HealthComponent>();

            foreach (var entity in _filter)
            {
                var targetEntityComponent = targetEntityComponentPool.Get(entity);

                if (targetEntityComponent.PackedEntity.Unpack(world, out int targetEntity))
                {
                    if (!healthComponentPool.Has(targetEntity))
                    {
                        continue;
                    }

                    var damageComponent = damageComponentPool.Get(entity);

                    ref var targetHealthComponent = ref healthComponentPool.Get(targetEntity);
                    targetHealthComponent.Current -= damageComponent.Amount;
                }

                world.DelEntity(entity);
            }
        }
    }
}
