using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class AbilitySystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<ITimeService> _timeService;
        private readonly EcsCustomInject<IAbilityService> _abilityService;

        private EcsFilter _abilityRequestFilter;
        private EcsFilter _abilityFilter;
        private EcsFilter _abilityFinishFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessAbilityUseRequests(world);
            ProcessAbilities(world);
            ProcessAbilitiesFinish(world);
        }

        private void ProcessAbilityUseRequests(EcsWorld world)
        {
            if (_abilityRequestFilter == null)
            {
                _abilityRequestFilter = world
                    .Filter<AbilityUseRequestComponent>()
                    .Inc<AbilitySetComponent>()
                    .End();
            }

            var abilityUseRequestComponentPool = world.GetPool<AbilityUseRequestComponent>();
            var abilityInUsageComponentPool = world.GetPool<AbilityInUsageComponent>();

            foreach (var entity in _abilityRequestFilter)
            {
                var abilityUseRequestComponent = abilityUseRequestComponentPool.Get(entity);

                // Check conditions

                // Pay price...

                // Start ability
                ref var abilityInUsageComponent = ref abilityInUsageComponentPool.Add(entity);
                abilityInUsageComponent.AbilityIndex = abilityUseRequestComponent.AbilityIndex;
                abilityInUsageComponent.StartTime = _timeService.Value.TimeSinceStart;
                abilityInUsageComponent.NextStepIndex = 0;

                abilityUseRequestComponentPool.Del(entity);
            }
        }

        private void ProcessAbilities(EcsWorld world)
        {
            if (_abilityFilter == null)
            {
                _abilityFilter = world
                    .Filter<AbilityInUsageComponent>()
                    .Inc<AbilitySetComponent>()
                    .End();
            }

            var abilityInUsageComponentPool = world.GetPool<AbilityInUsageComponent>();
            var abilitySetComponentPool = world.GetPool<AbilitySetComponent>();
            var animationTriggerComponentPool = world.GetPool<AnimationTriggerComponent>();
            var targetEntityComponentPool = world.GetPool<TargetEntityComponent>();
            var actionFlagPool = world.GetPool<ActionFlag>();
            var damageComponentPool = world.GetPool<DamageComponent>();
            var projectileFlagPool = world.GetPool<ProjectileFlag>();
            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            var movementComponentPool = world.GetPool<MovementComponent>();

            foreach (var entity in _abilityFilter)
            {
                ref var abilityInUsageComponent = ref abilityInUsageComponentPool.Get(entity);
                var abilitySetComponent = abilitySetComponentPool.Get(entity);

                var ability = _abilityService.Value.GetAbility(abilitySetComponent.AbilitySet[abilityInUsageComponent.AbilityIndex].AbilityId);
                if (abilityInUsageComponent.NextStepIndex >= ability.Steps.Length)
                {
                    continue;
                }

                var step = ability.Steps[abilityInUsageComponent.NextStepIndex];

                if (abilityInUsageComponent.StartTime + step.Time <= _timeService.Value.TimeSinceStart)
                {
                    abilityInUsageComponent.NextStepIndex++;

                    foreach (var action in step.Actions)
                    {
                        switch (action)
                        {
                            case AnimationTriggerAbilityAction animationTriggerAbilityAction:
                                ref var animationTriggerComponent = ref animationTriggerComponentPool.Add(entity);
                                animationTriggerComponent.Name = animationTriggerAbilityAction.TriggerName;

                                break;

                            case DamageAbilityAction damageAbilityAction:
                                int damageEntity = world.NewEntity();

                                actionFlagPool.Add(damageEntity);

                                ref var damageComponent = ref damageComponentPool.Add(damageEntity);
                                damageComponent.Amount = damageAbilityAction.Amount;

                                targetEntityComponentPool.Copy(entity, damageEntity);

                                break;

                            case ProjectileLaunchAbilityAction projectileLaunchAbilityAction:
                                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);
                                var transform = gameObjectUnityRefComponent.GameObject.transform;

                                int projectileEntity = world.NewEntity();
                                projectileFlagPool.Add(projectileEntity);

                                ref var projectileGameObjectUnityRefComponent = ref gameObjectUnityRefComponentPool.Add(projectileEntity);
                                projectileGameObjectUnityRefComponent.GameObject = GameObject.Instantiate(projectileLaunchAbilityAction.Projectile.Prefab, transform.position + Vector3.up * 1.65f, transform.rotation);

                                ref var projectileDamageComponent = ref damageComponentPool.Add(projectileEntity);
                                projectileDamageComponent.Amount = projectileLaunchAbilityAction.Damage;

                                ref var projectilMovementComponent = ref movementComponentPool.Add(projectileEntity);
                                projectilMovementComponent.Speed = projectileLaunchAbilityAction.Speed;

                                targetEntityComponentPool.Copy(entity, projectileEntity);

                                break;
                        }
                    }
                }
            }
        }

        private void ProcessAbilitiesFinish(EcsWorld world)
        {
            if (_abilityFinishFilter == null)
            {
                _abilityFinishFilter = world
                    .Filter<AbilityInUsageComponent>()
                    .Inc<AbilitySetComponent>()
                    .End();
            }

            var abilityInUsageComponentPool = world.GetPool<AbilityInUsageComponent>();
            var abilitySetComponentPool = world.GetPool<AbilitySetComponent>();

            foreach (var entity in _abilityFinishFilter)
            {
                var abilityInUsageComponent = abilityInUsageComponentPool.Get(entity);
                var abilitySetComponent = abilitySetComponentPool.Get(entity);

                var ability = _abilityService.Value.GetAbility(abilitySetComponent.AbilitySet[abilityInUsageComponent.AbilityIndex].AbilityId);

                if (_timeService.Value.TimeSinceStart >= abilityInUsageComponent.StartTime + ability.Duration)
                {
                    abilitySetComponent.AbilitySet[abilityInUsageComponent.AbilityIndex].TimeLastUsed = _timeService.Value.TimeSinceStart;

                    abilityInUsageComponentPool.Del(entity);
                }
            }
        }
    }
}
