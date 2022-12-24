using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;

namespace UnicornOne.Ecs.Systems
{
    internal class DamageSystem : IEcsRunSystem
    {
        EcsFilter _attackFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_attackFilter == null)
            {
                _attackFilter = world
                    .Filter<DamageComponent>()
                    .Inc<TargetComponent>()
                    .End();
            }

            var damagePool = world.GetPool<DamageComponent>();
            var targetPool = world.GetPool<TargetComponent>();
            var healthPool = world.GetPool<HealthComponent>();

            foreach (var entity in _attackFilter)
            {
                ref var damageComponent = ref damagePool.Get(entity);
                ref var targetComponent = ref targetPool.Get(entity);

                int targetEntity;
                if (targetComponent.TargetEntity.Unpack(world, out targetEntity))
                {
                    if (healthPool.Has(targetEntity))
                    {
                        ref var healthComponent = ref healthPool.Get(targetEntity);

                        healthComponent.CurrentHealth -= damageComponent.Damage;
                    }
                }

                world.DelEntity(entity);
            }
        }
    }
}
