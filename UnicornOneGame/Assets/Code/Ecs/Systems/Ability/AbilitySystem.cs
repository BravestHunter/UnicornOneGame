using System;
using Leopotam.EcsLite;
using UnicornOne.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    public class AbilitySystem : IEcsRunSystem
    {
        private EcsFilter _abilityUseFinishFilter;
        private EcsFilter _abilityUseRequestFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessAbilityUseFinish(world);
            ProcessAbilityUseRequest(world);
        }

        private void ProcessAbilityUseFinish(EcsWorld world)
        {
            if (_abilityUseFinishFilter == null)
            {
                _abilityUseFinishFilter = world
                    .Filter<AbilityInUsageComponent>()
                    .Exc<RequestedAnimatorStateComponent>()
                    .End();
            }

            var abilityInUsageComponentPool = world.GetPool<AbilityInUsageComponent>();
            var attackRechargePool = world.GetPool<AttackRechargeComponent>();

            foreach (var entity in _abilityUseFinishFilter)
            {
                abilityInUsageComponentPool.Del(entity);

                ref var attackRechargeComponent = ref attackRechargePool.Add(entity);
                attackRechargeComponent.LastAttackTime = Time.timeSinceLevelLoad;
            }
        }

        private void ProcessAbilityUseRequest(EcsWorld world)
        {
            if (_abilityUseRequestFilter == null)
            {
                _abilityUseRequestFilter = world
                    .Filter<AbilityUseRequest>()
                    .Exc<AbilityInUsageComponent>()
                    .End();
            }

            var abilityUseRequestPool = world.GetPool<AbilityUseRequest>();
            var abilityInUsageComponentPool = world.GetPool<AbilityInUsageComponent>();
            var animatorTriggerRequestPool = world.GetPool<AnimatorTriggerRequest>();

            // Temp
            var targetPool = world.GetPool<TargetComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();

            foreach (var entity in _abilityUseRequestFilter)
            {
                var abilityUseRequest = abilityUseRequestPool.Get(entity);

                ref var abilityInUsageComponent = ref abilityInUsageComponentPool.Add(entity);
                abilityInUsageComponent.Ability = abilityUseRequest.Ability;

                ref var animatorTriggerRequest = ref animatorTriggerRequestPool.Add(entity);
                animatorTriggerRequest.Name = abilityUseRequest.Ability.Name;

                // Temp
                ref var targetComponent = ref targetPool.Get(entity);
                ref var gameObjectRefComponent = ref gameObjectRefPool.Get(entity);
                int targetEntity;
                if (targetComponent.TargetEntity.Unpack(world, out targetEntity))
                {
                    gameObjectRefComponent.GameObject.transform.LookAt(gameObjectRefPool.Get(targetEntity).GameObject.transform.position);
                }

                abilityUseRequestPool.Del(entity);
            }
        }
    }
}
