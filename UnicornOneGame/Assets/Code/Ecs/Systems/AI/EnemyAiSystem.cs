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
    internal class EnemyAiSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<AbilityService> _abilityService;

        private EcsFilter _enemyAiFilter;
        private EcsFilter _heroFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_enemyAiFilter == null)
            {
                _enemyAiFilter = world
                   .Filter<EnemyBehaviorAiComponent>()
                   .Inc<NavigationComponent>()
                   .Inc<GameObjectUnityRefComponent>()
                   .End();
            }

            var enemyBehaviorAiPool = world.GetPool<EnemyBehaviorAiComponent>();
            var navigationPool = world.GetPool<NavigationComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();

            var targetPool = world.GetPool<TargetComponent>();
            var standPool = world.GetPool<StandFlag>();

            var abilityUseRequestPool = world.GetPool<AbilityUseRequest>();
            var abilityInUsageComponentPool = world.GetPool<AbilityInUsageComponent>();
            var abilityRechargePool = world.GetPool<AbilityRechargeComponent>();

            var abilitySetPool = world.GetPool<AbilitySetComponent>();

            Dictionary<int, Vector3> heroPositions = null;

            foreach (var entity in _enemyAiFilter)
            {
                ref var enemyBehaviorAiComponent = ref enemyBehaviorAiPool.Get(entity);
                var gameObjectRefComponent = gameObjectRefPool.Get(entity);

                Vector3 entityPosition = gameObjectRefComponent.GameObject.transform.position;

                switch (enemyBehaviorAiComponent.CurrentState)
                {
                    case EnemyBehaviorAiComponent.State.SearchingForTarget:
                        {
                            if (heroPositions == null)
                            {
                                heroPositions = GetHeroPositions(world);
                            }

                            // Case: No possible targets
                            if (heroPositions.Count == 0)
                            {
                                break;
                            }

                            // Set closest hero as target
                            //var closestTarget = heroPositions.OrderBy(pair => (entityPosition - pair.Value).sqrMagnitude).First();

                            // Set first hero as target
                            var closestTarget = heroPositions.First();

                            enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SelectingAbility;
                            ref var targetComponent = ref targetPool.Add(entity);
                            targetComponent.TargetEntity = world.PackEntity(closestTarget.Key);

                            break;
                        }

                    case EnemyBehaviorAiComponent.State.SelectingAbility:
                        {
                            // Case: No target
                            if (!targetPool.Has(entity))
                            {
                                enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SearchingForTarget;

                                break;
                            }

                            // Case: Target is not alive
                            ref var targetComponent = ref targetPool.Get(entity);
                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                // Target is missing, search for new one
                                enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SearchingForTarget;
                                targetPool.Del(entity);

                                break;
                            }

                            // Select ability
                            var abilitySetComponent = abilitySetPool.Get(entity);
                            var abilitySet = _abilityService.Value.GetAbilitySet(abilitySetComponent.Index);
                            var ability = abilitySet.Abilities.Last();

                            enemyBehaviorAiComponent.SelectedAbility = ability;
                            enemyBehaviorAiComponent.SelectedAbilityIndex = 0;
                            enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.MovingToTarget;

                            break;
                        }

                    case EnemyBehaviorAiComponent.State.MovingToTarget:
                        {
                            // Case: No target
                            if (!targetPool.Has(entity))
                            {
                                enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SearchingForTarget;

                                break;
                            }

                            // Case: Target is not alive
                            ref var targetComponent = ref targetPool.Get(entity);
                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                // Target is missing, search for new one
                                enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SearchingForTarget;
                                targetPool.Del(entity);

                                break;
                            }

                            ref var navigationComponent = ref navigationPool.Get(entity);

                            // Case: Too far from target, keep moving
                            Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;
                            if ((targetEntityPosition - entityPosition).magnitude > enemyBehaviorAiComponent.SelectedAbility.Range)
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
                            enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.AttackingWithAbility;

                            break;
                        }

                    case EnemyBehaviorAiComponent.State.AttackingWithAbility:
                        {
                            // Case: Attack is happening
                            if (abilityInUsageComponentPool.Has(entity))
                            {
                                break;
                            }

                            // Case: No target
                            if (!targetPool.Has(entity))
                            {
                                enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SearchingForTarget;

                                break;
                            }

                            // Case: Target is not alive
                            ref var targetComponent = ref targetPool.Get(entity);
                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SearchingForTarget;
                                targetPool.Del(entity);

                                break;
                            }

                            // Case: Target is too far
                            Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;
                            if ((entityPosition - targetEntityPosition).magnitude > enemyBehaviorAiComponent.SelectedAbility.Range)
                            {
                                enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.MovingToTarget;

                                break;
                            }

                            // Hero should stand now
                            if (!standPool.Has(entity))
                            {
                                standPool.Add(entity);
                            }

                            // Case: Ability recharge is happening
                            var abilityRechargeComponent = abilityRechargePool.Get(entity);
                            float timePassed = Time.timeSinceLevelLoad - abilityRechargeComponent.LastUseTimes[enemyBehaviorAiComponent.SelectedAbilityIndex];
                            if (timePassed < enemyBehaviorAiComponent.SelectedAbility.Cooldown)
                            {
                                break;
                            }

                            // Use ability
                            ref var abilityUseRequest = ref abilityUseRequestPool.Add(entity);
                            abilityUseRequest.Ability = enemyBehaviorAiComponent.SelectedAbility;
                            abilityUseRequest.AbilityIndex = enemyBehaviorAiComponent.SelectedAbilityIndex;

                            break;
                        }
                }
            }
        }

        private Dictionary<int, Vector3> GetHeroPositions(EcsWorld world)
        {
            if (_heroFilter == null)
            {
                _heroFilter = world
                    .Filter<HeroFlag>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>();

            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();

            foreach (var entity in _heroFilter)
            {
                ref var gameObjectRefComponent = ref gameObjectRefPool.Get(entity);

                positions.Add(entity, gameObjectRefComponent.GameObject.transform.position);
            }

            return positions;
        }
    }
}
