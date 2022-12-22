using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;

namespace UnicornOne.Ecs.Systems
{
    internal class AttackSystem : IEcsRunSystem
    {
        EcsFilter _attackFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_attackFilter == null)
            {
                _attackFilter = world
                    .Filter<AttackComponent>()
                    .Inc<TargetComponent>()
                    .End();
            }

            var attackPool = world.GetPool<AttackComponent>();
            var targetPool = world.GetPool<TargetComponent>();
            var healthPool = world.GetPool<HealthComponent>();

            foreach (var entity in _attackFilter)
            {
                ref var attackComponent = ref attackPool.Get(entity);
                ref var targetComponent = ref targetPool.Get(entity);

                int targetEntity;
                if (targetComponent.TargetEntity.Unpack(world, out targetEntity))
                {
                    if (healthPool.Has(targetEntity))
                    {
                        ref var healthComponent = ref healthPool.Get(targetEntity);

                        healthComponent.CurrentHealth -= attackComponent.Damage;
                    }
                }

                world.DelEntity(entity);
            }
        }
    }
}
