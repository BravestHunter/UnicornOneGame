using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class ProjectileSystem : IEcsRunSystem
    {
        private const float Epsilon = 0.1f;

        private EcsFilter _hitFilter;
        private EcsFilter _targetPositionUpdateFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessTargetPositionUpdates(world);
            ProcessHits(world);
        }

        private void ProcessTargetPositionUpdates(EcsWorld world)
        {
            if (_targetPositionUpdateFilter == null)
            {
                _targetPositionUpdateFilter = world
                    .Filter<ProjectileFlag>()
                    .Inc<TargetEntityComponent>()
                    .Inc<GameObjectUnityRefComponent>()
                    .Inc<TargetPositionMoveComponent>()
                    .End();
            }

            var targetEntityComponentPool = world.GetPool<TargetEntityComponent>();
            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            var targetPositionMoveComponentPool = world.GetPool<TargetPositionMoveComponent>();

            foreach (var entity in _targetPositionUpdateFilter)
            {
                var targetEntityComponent = targetEntityComponentPool.Get(entity);
                int targetEntity;
                if (!targetEntityComponent.PackedEntity.Unpack(world, out targetEntity))
                {
                    world.DelEntity(entity);

                    continue;
                }

                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);
                var targetGameObjectUnityRefComponentPool = gameObjectUnityRefComponentPool.Get(targetEntity);

                ref var targetPositionMoveComponent = ref targetPositionMoveComponentPool.Get(entity);
                targetPositionMoveComponent.Position = targetGameObjectUnityRefComponentPool.GameObject.transform.position;
                targetPositionMoveComponent.Position.y = gameObjectUnityRefComponent.GameObject.transform.position.y;
            }
        }

        private void ProcessHits(EcsWorld world)
        {
            if (_hitFilter == null)
            {
                _hitFilter = world
                    .Filter<ProjectileFlag>()
                    .Inc<TargetEntityComponent>()
                    .Inc<GameObjectUnityRefComponent>()
                    .Exc<TargetPositionMoveComponent>()
                    .End();
            }

            var targetEntityComponentPool = world.GetPool<TargetEntityComponent>();
            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            var targetPositionMoveComponentPool = world.GetPool<TargetPositionMoveComponent>();
            var actionFlagPool = world.GetPool<ActionFlag>();
            var damageComponentPool = world.GetPool<DamageComponent>();

            foreach (var entity in _hitFilter)
            {
                var targetEntityComponent = targetEntityComponentPool.Get(entity);
                int targetEntity;
                if (!targetEntityComponent.PackedEntity.Unpack(world, out targetEntity))
                {
                    world.DelEntity(entity);

                    continue;
                }

                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);
                var targetGameObjectUnityRefComponentPool = gameObjectUnityRefComponentPool.Get(targetEntity);

                Vector3 targetFlatPosition = targetGameObjectUnityRefComponentPool.GameObject.transform.position;
                targetFlatPosition.y = gameObjectUnityRefComponent.GameObject.transform.position.y;

                if ((gameObjectUnityRefComponent.GameObject.transform.position - targetFlatPosition).sqrMagnitude > Epsilon)
                {
                    // Set target position
                    ref var targetPositionMoveComponent = ref targetPositionMoveComponentPool.Add(entity);
                    targetPositionMoveComponent.Position = targetFlatPosition;

                    continue;
                }

                // Process hit
                int damageEntity = world.NewEntity();

                actionFlagPool.Add(damageEntity);

                damageComponentPool.Copy(entity, damageEntity);
                targetEntityComponentPool.Copy(entity, damageEntity);

                world.DelEntity(entity);
            }
        }
    }
}
