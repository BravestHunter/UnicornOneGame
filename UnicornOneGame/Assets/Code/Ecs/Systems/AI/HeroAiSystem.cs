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

namespace UnicornOne.Ecs.Systems
{
    internal class HeroAiSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<AbilityService> _abilityService;

        private EcsFilter _squadAiFilter;
        private EcsFilter _heroFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            TargetComponent? squadTargetComponent = GetSquadTarget(world);

            if (_heroFilter == null)
            {
                _heroFilter = world
                    .Filter<HeroFlag>()
                    .Inc<HeroBehaviorAiComponent>()
                    .Inc<NavigationComponent>()
                    .Inc<GameObjectUnityRefComponent>()
                    .Inc<AbilitySetComponent>()
                    .End();
            }

            var heroBehaviorAiPool = world.GetPool<HeroBehaviorAiComponent>();
            var navigationPool = world.GetPool<NavigationComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();
            var targetPool = world.GetPool<TargetComponent>();
            var abilityUseRequestPool = world.GetPool<AbilityUseRequest>();
            var abilityInUsageComponentPool = world.GetPool<AbilityInUsageComponent>();
            var attackRechargePool = world.GetPool<AttackRechargeComponent>();
            var standPool = world.GetPool<StandFlag>();

            var abilitySetPool = world.GetPool<AbilitySetComponent>();

            foreach (var entity in _heroFilter)
            {
                ref var heroBehaviorAiComponent = ref heroBehaviorAiPool.Get(entity);
                ref var navigationComponent = ref navigationPool.Get(entity);
                ref var gameObjectRefComponent = ref gameObjectRefPool.Get(entity);

                Vector3 entityPosition = gameObjectRefComponent.GameObject.transform.position;

                switch (heroBehaviorAiComponent.CurrentState)
                {
                    case HeroBehaviorAiComponent.State.SearchingForTarget:
                        {
                            // Case: Squad leader has no target
                            if (squadTargetComponent == null)
                            {
                                break;
                            }

                            // Set target of squad leader
                            heroBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.SelectingAbility;

                            ref var targetComponent = ref targetPool.Add(entity);
                            targetComponent.TargetEntity = squadTargetComponent.Value.TargetEntity;

                            break;
                        }

                    case HeroBehaviorAiComponent.State.SelectingAbility:
                        {
                            // Case: No target
                            if (!targetPool.Has(entity))
                            {
                                heroBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.SearchingForTarget;

                                break;
                            }

                            // Case: Target is not alive
                            ref var targetComponent = ref targetPool.Get(entity);
                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                // Target is missing, search for new one
                                heroBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.SearchingForTarget;
                                targetPool.Del(entity);

                                break;
                            }

                            // Select ability
                            var abilitySetComponent = abilitySetPool.Get(entity);
                            var abilitySet = _abilityService.Value.GetAbilitySet(abilitySetComponent.Index);
                            var ability = abilitySet.Abilities.Last();

                            heroBehaviorAiComponent.SelectedAbility = ability;
                            heroBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.MovingToTarget;

                            break;
                        }

                    case HeroBehaviorAiComponent.State.MovingToTarget:
                        {
                            // Case: No target
                            if (!targetPool.Has(entity))
                            {
                                heroBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.SearchingForTarget;

                                break;
                            }

                            // Case: Target is not alive
                            ref var targetComponent = ref targetPool.Get(entity);
                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                // Target is missing, search for new one
                                heroBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.SearchingForTarget;
                                targetPool.Del(entity);

                                break;
                            }

                            // Case: Too far from target, keep moving
                            Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;
                            if ((targetEntityPosition - entityPosition).magnitude > heroBehaviorAiComponent.SelectedAbility.Range)
                            {
                                navigationComponent.DestionationPosition = targetEntityPosition;

                                if (standPool.Has(entity))
                                {
                                    standPool.Del(entity);
                                }

                                break;
                            }
                            else
                            {
                                navigationComponent.DestionationPosition = entityPosition;
                            }

                            // Reached target point
                            heroBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.AttackingWithAbility;

                            break;
                        }

                    case HeroBehaviorAiComponent.State.AttackingWithAbility:
                        {
                            // Case: Ability is in usage
                            if (abilityInUsageComponentPool.Has(entity))
                            {
                                break;
                            }

                            // Case: No target
                            if (!targetPool.Has(entity))
                            {
                                heroBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.SearchingForTarget;

                                break;
                            }

                            // Case: Target is not alive
                            ref var targetComponent = ref targetPool.Get(entity);
                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                heroBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.SearchingForTarget;
                                targetPool.Del(entity);

                                break;
                            }

                            // Case: Target is too far
                            Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;
                            if ((entityPosition - targetEntityPosition).magnitude > heroBehaviorAiComponent.SelectedAbility.Range)
                            {
                                heroBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.MovingToTarget;

                                break;
                            }

                            // Hero should stand now
                            if (!standPool.Has(entity))
                            {
                                standPool.Add(entity);
                            }

                            // Case: Attack recharge is happening
                            if (attackRechargePool.Has(entity))
                            {
                                break;
                            }

                            // Use ability
                            ref var abilityUseRequest = ref abilityUseRequestPool.Add(entity);
                            abilityUseRequest.Ability = heroBehaviorAiComponent.SelectedAbility;

                            break;
                        }
                }
            }
        }

        private TargetComponent? GetSquadTarget(EcsWorld world)
        {
            if (_squadAiFilter == null)
            {
                _squadAiFilter = world
                    .Filter<SquadAiFlag>()
                    .Inc<TargetComponent>()
                    .End();
            }

            TargetComponent? target = null;

            var targetPool = world.GetPool<TargetComponent>();

            foreach (var entity in _squadAiFilter)
            {
                target = targetPool.Get(entity);
            }

            return target;
        }
    }
}
