using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Components.Flags;

namespace UnicornOne.Ecs.Systems
{
    internal class AttackSystem : IEcsRunSystem
    {
        private EcsFilter _attackRequestFilter;
        private EcsFilter _busyAttackRequestFilter;
        private EcsFilter _hitFilter;
        private EcsFilter _attackFinishFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessAttackFinish(world);
            ProcessAttackRequest(world);
            ProcessHit(world);
        }

        private void ProcessAttackFinish(EcsWorld world)
        {
            if (_attackFinishFilter == null)
            {
                _attackFinishFilter = world
                    .Filter<AttackFlag>()
                    .Exc<AttackAnimationFlag>()
                    .End();
            }

            var attackFlagPool = world.GetPool<AttackFlag>();

            foreach (var entity in _attackFinishFilter)
            {
                attackFlagPool.Del(entity);
            }
        }

        private void ProcessAttackRequest(EcsWorld world)
        {
            if (_attackRequestFilter == null)
            {
                _attackRequestFilter = world
                    .Filter<AttackRequest>()
                    .Exc<AttackFlag>()
                    .End();
            }

            if (_busyAttackRequestFilter == null)
            {
                _busyAttackRequestFilter = world
                    .Filter<AttackRequest>()
                    .Inc<AttackFlag>()
                    .End();
            }

            var attackRequestPool = world.GetPool<AttackRequest>();
            var attackFlagPool = world.GetPool<AttackFlag>();
            var attackAnimationRequestPool = world.GetPool<AttackAnimationRequest>();

            foreach (var entity in _attackRequestFilter)
            {
                attackAnimationRequestPool.Add(entity);
                attackFlagPool.Add(entity);
            }

            foreach (var entity in _busyAttackRequestFilter)
            {
                attackRequestPool.Del(entity);
            }
        }

        private void ProcessHit(EcsWorld world)
        {
            if (_hitFilter == null)
            {
                _hitFilter = world
                    .Filter<HitRequest>()
                    .Inc<TargetComponent>()
                    .Inc<MeleeAtackParametersComponent>()
                    .End();
            }

            var hitRequestPool = world.GetPool<HitRequest>();
            var targetPool = world.GetPool<TargetComponent>();
            var meleeAtackParametersPool = world.GetPool<MeleeAtackParametersComponent>();
            var damagePool = world.GetPool<DamageComponent>();

            foreach (var entity in _hitFilter)
            {
                ref var targetComponent = ref targetPool.Get(entity);
                ref var meleeAtackParametersComponent = ref meleeAtackParametersPool.Get(entity);

                {
                    var damageEntity = world.NewEntity();

                    ref var damageTargetComponent = ref targetPool.Add(damageEntity);
                    damageTargetComponent.TargetEntity = targetComponent.TargetEntity;

                    ref var damageComponent = ref damagePool.Add(damageEntity);
                    damageComponent.Damage = meleeAtackParametersComponent.Damage;
                }

                hitRequestPool.Del(entity);
            }
        }
    }
}
