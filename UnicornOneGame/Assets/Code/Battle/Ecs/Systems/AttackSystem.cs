using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class AttackSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<ITimeService> _timeService;

        private EcsFilter _attackFilter;
        private EcsFilter _attackCooldownFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessAttacks(world);
            ProcessCooldowns(world);
        }

        private void ProcessAttacks(EcsWorld world)
        {
            if (_attackFilter == null)
            {
                _attackFilter = world
                    .Filter<AttackComponent>()
                    .Inc<TargetEntityComponent>()
                    .Exc<AttackInCooldownComponent>()
                    .End();
            }

            var attackComponentPool = world.GetPool<AttackComponent>();
            var targetEntityComponentPool = world.GetPool<TargetEntityComponent>();
            var damageComponentPool = world.GetPool<DamageComponent>();
            var animationTriggerComponentPool = world.GetPool<AnimationTriggerComponent>();
            var attackInCooldownComponentPool = world.GetPool<AttackInCooldownComponent>();

            foreach (var entity in _attackFilter)
            {
                var attackComponent = attackComponentPool.Get(entity);

                int damageEntity = world.NewEntity();

                ref var damageComponent = ref damageComponentPool.Add(damageEntity);
                damageComponent.Amount = attackComponent.Damage;

                targetEntityComponentPool.Copy(entity, damageEntity);

                ref var animationTriggerComponent = ref animationTriggerComponentPool.Add(entity);
                animationTriggerComponent.Name = "AttackTrigger";

                ref var attackInCooldownComponent = ref attackInCooldownComponentPool.Add(entity);
                attackInCooldownComponent.EndTime = _timeService.Value.TimeSinceStart + attackComponent.Cooldown;

                attackComponentPool.Del(entity);
            }
        }

        private void ProcessCooldowns(EcsWorld world)
        {
            if (_attackCooldownFilter == null)
            {
                _attackCooldownFilter = world
                    .Filter<AttackInCooldownComponent>()
                    .End();
            }

            var attackInCooldownComponentPool = world.GetPool<AttackInCooldownComponent>();

            foreach (var entity in _attackCooldownFilter)
            {
                var attackInCooldownComponent = attackInCooldownComponentPool.Get(entity);

                if (attackInCooldownComponent.EndTime <= _timeService.Value.TimeSinceStart)
                {
                    attackInCooldownComponentPool.Del(entity);
                }
            }
        }
    }
}
