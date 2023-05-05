using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Components.Ability;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.ScriptableObjects;

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
                abilityInUsageComponent.AbilityId = abilityUseRequestComponent.AbilityId;
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
                    .End();
            }

            var abilityInUsageComponentPool = world.GetPool<AbilityInUsageComponent>();
            var animationTriggerComponentPool = world.GetPool<AnimationTriggerComponent>();
            var targetEntityComponentPool = world.GetPool<TargetEntityComponent>();
            var damageComponentPool = world.GetPool<DamageComponent>();

            foreach (var entity in _abilityFilter)
            {
                ref var abilityInUsageComponent = ref abilityInUsageComponentPool.Get(entity);

                var ability = _abilityService.Value.GetAbility(abilityInUsageComponent.AbilityId);
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

                                ref var damageComponent = ref damageComponentPool.Add(damageEntity);
                                damageComponent.Amount = damageAbilityAction.Amount;

                                targetEntityComponentPool.Copy(entity, damageEntity);

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

                var ability = _abilityService.Value.GetAbility(abilityInUsageComponent.AbilityId);

                if (_timeService.Value.TimeSinceStart >= abilityInUsageComponent.StartTime + ability.Duration)
                {
                    ref var abilitySetComponent = ref abilitySetComponentPool.Get(entity);
                    abilitySetComponent.AbilitySet[0].TimeLastUsed = _timeService.Value.TimeSinceStart;

                    abilityInUsageComponentPool.Del(entity);
                }
            }
        }
    }
}
