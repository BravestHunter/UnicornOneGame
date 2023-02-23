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
        private EcsFilter _hitFilter;
        private EcsFilter _shotFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessHits(world);
            ProcessShots(world);
        }

        private void ProcessHits(EcsWorld world)
        {
            if (_hitFilter == null)
            {
                _hitFilter = world
                    .Filter<HitRequest>()
                    .Inc<TargetComponent>()
                    .Inc<AbilityInUsageComponent>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            var hitRequestPool = world.GetPool<HitRequest>();
            var targetPool = world.GetPool<TargetComponent>();
            var abilityInUsageComponentPool = world.GetPool<AbilityInUsageComponent>();
            var damagePool = world.GetPool<DamageComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();
            var effectFlagPool = world.GetPool<EffectFlag>();
            var effectLifeSpanPool = world.GetPool<EffectLifeSpanComponent>();

            foreach (var entity in _hitFilter)
            {
                var targetComponent = targetPool.Get(entity);
                var abilityInUsageComponent = abilityInUsageComponentPool.Get(entity);
                var gameObjectRefComponent = gameObjectRefPool.Get(entity);

                int targetEntity;
                if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                {
                    continue;
                }

                var damageEntity = world.NewEntity();

                ref var damageTargetComponent = ref targetPool.Add(damageEntity);
                damageTargetComponent.TargetEntity = targetComponent.TargetEntity;

                ref var damageComponent = ref damagePool.Add(damageEntity);
                damageComponent.Damage = abilityInUsageComponent.Ability.Damage;

                if (abilityInUsageComponent.Ability.Effect != null)
                {
                    var effect = abilityInUsageComponent.Ability.Effect;

                    var effectEntity = world.NewEntity();

                    effectFlagPool.Add(effectEntity);

                    ref var effectLifeSpanComponent = ref effectLifeSpanPool.Add(effectEntity);
                    effectLifeSpanComponent.LifeSpan = 0.25f;
                    effectLifeSpanComponent.CreationTime = Time.timeSinceLevelLoad;

                    var effectGameObject = GameObject.Instantiate(effect.Prefab.Prefab);

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

        private void ProcessShots(EcsWorld world)
        {
            if (_shotFilter == null)
            {
                _shotFilter = world
                    .Filter<ShotRequest>()
                    .Inc<TargetComponent>()
                    .Inc<AbilityInUsageComponent>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            var shotRequestPool = world.GetPool<ShotRequest>();
            var targetPool = world.GetPool<TargetComponent>();
            var abilityInUsageComponentPool = world.GetPool<AbilityInUsageComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();
            var projectileParametersPool = world.GetPool<ProjectileParametersComponent>();
            var effectFlagPool = world.GetPool<EffectFlag>();
            var effectLifeSpanPool = world.GetPool<EffectLifeSpanComponent>();

            foreach (var entity in _shotFilter)
            {
                var targetComponent = targetPool.Get(entity);
                var abilityInUsageComponent = abilityInUsageComponentPool.Get(entity);
                var gameObjectRefComponent = gameObjectRefPool.Get(entity);

                int targetEntity;
                if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                {
                    continue;
                }

                if (abilityInUsageComponent.Ability.Projectile != null)
                {
                    var projectile = abilityInUsageComponent.Ability.Projectile;

                    // Spawn projectile
                    var projectileEntity = world.NewEntity();

                    ref var projectileTargetComponent = ref targetPool.Add(projectileEntity);
                    projectileTargetComponent.TargetEntity = targetComponent.TargetEntity;

                    ref var projectileParametersComponent = ref projectileParametersPool.Add(projectileEntity);
                    projectileParametersComponent.Damage = abilityInUsageComponent.Ability.Damage;
                    projectileParametersComponent.MoveSpeed = projectile.MoveInfo.Speed;

                    var projectileGameObject = GameObject.Instantiate(projectile.PrefabInfo.Prefab);
                    projectileGameObject.transform.position = gameObjectRefComponent.GameObject.transform.position + Vector3.up * 1.65f + gameObjectRefComponent.GameObject.transform.forward * 1.0f;
                    projectileGameObject.transform.rotation = gameObjectRefComponent.GameObject.transform.rotation;

                    ref var projectileGameObjectRefComponent = ref gameObjectRefPool.Add(projectileEntity);
                    projectileGameObjectRefComponent.GameObject = projectileGameObject;
                }

                if (abilityInUsageComponent.Ability.Effect != null)
                {
                    var effect = abilityInUsageComponent.Ability.Effect;

                    var effectEntity = world.NewEntity();

                    effectFlagPool.Add(effectEntity);

                    ref var effectLifeSpanComponent = ref effectLifeSpanPool.Add(effectEntity);
                    effectLifeSpanComponent.LifeSpan = 0.25f;
                    effectLifeSpanComponent.CreationTime = Time.timeSinceLevelLoad;

                    var effectGameObject = GameObject.Instantiate(effect.Prefab.Prefab);

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

                shotRequestPool.Del(entity);
            }
        }
    }
}
