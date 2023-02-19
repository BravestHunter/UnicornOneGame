using DigitalRuby.LightningBolt;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Services;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace UnicornOne.Ecs.Systems
{
    internal class AttackSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<ProjectileService> _projectileService;
        private readonly EcsCustomInject<EffectService> _effectService;

        private EcsFilter _attackFinishFilter;
        private EcsFilter _attackRequestFilter;
        private EcsFilter _busyAttackRequestFilter;
        private EcsFilter _hitFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessAttackFinish(world);
            ProcessAttackRequest(world);
            CleanBusyAttackRequests(world);
            ProcessHits(world);
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
                    .Inc<GameObjectUnityRefComponent>()
                    .Exc<AttackFlag>()
                    .End();
            }

            var attackRequestPool = world.GetPool<AttackRequest>();
            var attackFlagPool = world.GetPool<AttackFlag>();
            var targetPool = world.GetPool<TargetComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();

            var animatorTriggerRequestPool = world.GetPool<AnimatorTriggerRequest>();

            foreach (var entity in _attackRequestFilter)
            {
                attackFlagPool.Add(entity);

                ref var animatorTriggerRequest = ref animatorTriggerRequestPool.Add(entity);
                animatorTriggerRequest.Name = "AttackTrigger";

                ref var targetComponent = ref targetPool.Get(entity);
                ref var gameObjectRefComponent = ref gameObjectRefPool.Get(entity);

                int targetEntity;
                if (targetComponent.TargetEntity.Unpack(world, out targetEntity))
                {
                    gameObjectRefComponent.GameObject.transform.LookAt(gameObjectRefPool.Get(targetEntity).GameObject.transform.position);
                }

                attackRequestPool.Del(entity);
            }
        }

        private void CleanBusyAttackRequests(EcsWorld world)
        {
            if (_busyAttackRequestFilter == null)
            {
                _busyAttackRequestFilter = world
                    .Filter<AttackRequest>()
                    .Inc<AttackFlag>()
                    .End();
            }

            var attackRequestPool = world.GetPool<AttackRequest>();

            foreach (var entity in _busyAttackRequestFilter)
            {
                attackRequestPool.Del(entity);
            }
        }

        private void ProcessHits(EcsWorld world)
        {
            if (_hitFilter == null)
            {
                _hitFilter = world
                    .Filter<HitRequest>()
                    .Inc<TargetComponent>()
                    .Inc<AtackParametersComponent>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            var hitRequestPool = world.GetPool<HitRequest>();
            var targetPool = world.GetPool<TargetComponent>();
            var atackParametersPool = world.GetPool<AtackParametersComponent>();
            var damagePool = world.GetPool<DamageComponent>();
            var rangedFlagPool = world.GetPool<RangedFlag>();
            var projectileParametersPool = world.GetPool<ProjectileParametersComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();

            var hasAttackEffectFlagPool = world.GetPool<HasAttackEffectFlag>();
            var effectFlagPool = world.GetPool<EffectFlag>();
            var effectLifeSpanPool = world.GetPool<EffectLifeSpanComponent>();

            foreach (var entity in _hitFilter)
            {
                var targetComponent = targetPool.Get(entity);
                var atackParametersComponent = atackParametersPool.Get(entity);
                var gameObjectRefComponent = gameObjectRefPool.Get(entity);

                int targetEntity;
                if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                {
                    continue;
                }

                if (rangedFlagPool.Has(entity))
                {
                    // Spawn projectile
                    var projectileEntity = world.NewEntity();

                    ref var projectileTargetComponent = ref targetPool.Add(projectileEntity);
                    projectileTargetComponent.TargetEntity = targetComponent.TargetEntity;

                    ref var projectileParametersComponent = ref projectileParametersPool.Add(projectileEntity);
                    projectileParametersComponent.Damage = atackParametersComponent.Damage;
                    projectileParametersComponent.MoveSpeed = _projectileService.Value.Projectile.MoveInfo.Speed;

                    var projectileGameObject = GameObject.Instantiate(_projectileService.Value.Projectile.PrefabInfo.Prefab);
                    projectileGameObject.transform.position = gameObjectRefComponent.GameObject.transform.position + Vector3.up * 1.65f + gameObjectRefComponent.GameObject.transform.forward * 1.0f;
                    projectileGameObject.transform.rotation = gameObjectRefComponent.GameObject.transform.rotation;

                    ref var projectileGameObjectRefComponent = ref gameObjectRefPool.Add(projectileEntity);
                    projectileGameObjectRefComponent.GameObject = projectileGameObject;
                }
                else
                {
                    var damageEntity = world.NewEntity();

                    ref var damageTargetComponent = ref targetPool.Add(damageEntity);
                    damageTargetComponent.TargetEntity = targetComponent.TargetEntity;

                    ref var damageComponent = ref damagePool.Add(damageEntity);
                    damageComponent.Damage = atackParametersComponent.Damage;
                }

                if (hasAttackEffectFlagPool.Has(entity))
                {
                    var effectEntity = world.NewEntity();

                    effectFlagPool.Add(effectEntity);

                    ref var effectLifeSpanComponent = ref effectLifeSpanPool.Add(effectEntity);
                    effectLifeSpanComponent.LifeSpan = 0.25f;
                    effectLifeSpanComponent.CreationTime = Time.timeSinceLevelLoad;

                    var effectGameObject = GameObject.Instantiate(_effectService.Value.Effect.PrefabInfo.Prefab);

                    ref var projectileGameObjectRefComponent = ref gameObjectRefPool.Add(effectEntity);
                    projectileGameObjectRefComponent.GameObject = effectGameObject;

                    Vector3 entityPosition = gameObjectRefComponent.GameObject.transform.position + Vector3.up * 1.65f + gameObjectRefComponent.GameObject.transform.forward * 1.0f;
                    Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position + Vector3.up * 1.65f;

                    LightningBoltScript script = effectGameObject.GetComponent<LightningBoltScript>();
                    script.StartObject = null;
                    script.StartPosition = entityPosition;
                    script.EndObject = null;
                    script.EndPosition = targetEntityPosition;
                }

                hitRequestPool.Del(entity);
            }
        }
    }
}
