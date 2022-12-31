using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Components.Flags;
using UnicornOne.Ecs.Components.Refs;
using UnityEngine;

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
            var attackRechargePool = world.GetPool<AttackRechargeComponent>();

            foreach (var entity in _attackFinishFilter)
            {
                attackFlagPool.Del(entity);

                ref var attackRechargeComponent = ref attackRechargePool.Add(entity);
                attackRechargeComponent.LastAttackTime = Time.timeSinceLevelLoad;
            }
        }

        private void ProcessAttackRequest(EcsWorld world)
        {
            if (_attackRequestFilter == null)
            {
                _attackRequestFilter = world
                    .Filter<AttackRequest>()
                    .Inc<TargetComponent>()
                    .Inc<GameObjectRefComponent>()
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
            var targetPool = world.GetPool<TargetComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectRefComponent>();

            foreach (var entity in _attackRequestFilter)
            {
                attackFlagPool.Add(entity);
                attackAnimationRequestPool.Add(entity);

                ref var targetComponent = ref targetPool.Get(entity);
                ref var gameObjectRefComponent = ref gameObjectRefPool.Get(entity);

                int targetEntity;
                if (targetComponent.TargetEntity.Unpack(world, out targetEntity))
                {
                    gameObjectRefComponent.GameObject.transform.LookAt(gameObjectRefPool.Get(targetEntity).GameObject.transform.position);
                }

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
                    .Inc<AtackParametersComponent>()
                    .End();
            }

            var hitRequestPool = world.GetPool<HitRequest>();
            var targetPool = world.GetPool<TargetComponent>();
            var atackParametersPool = world.GetPool<AtackParametersComponent>();
            var damagePool = world.GetPool<DamageComponent>();

            foreach (var entity in _hitFilter)
            {
                ref var targetComponent = ref targetPool.Get(entity);
                ref var atackParametersComponent = ref atackParametersPool.Get(entity);

                {
                    var damageEntity = world.NewEntity();

                    ref var damageTargetComponent = ref targetPool.Add(damageEntity);
                    damageTargetComponent.TargetEntity = targetComponent.TargetEntity;

                    ref var damageComponent = ref damagePool.Add(damageEntity);
                    damageComponent.Damage = atackParametersComponent.Damage;
                }

                hitRequestPool.Del(entity);
            }
        }
    }
}
