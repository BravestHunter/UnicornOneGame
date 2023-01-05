using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace UnicornOne.Ecs.Systems
{
    internal class EnemyAiSystem : IEcsRunSystem
    {
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

            var attackFlagPool = world.GetPool<AttackFlag>();
            var atackParametersPool = world.GetPool<AtackParametersComponent>();
            var attackRechargePool = world.GetPool<AttackRechargeComponent>();
            var attackRequestPool = world.GetPool<AttackRequest>();

            Dictionary<int, Vector3> heroPositions = null;

            foreach (var entity in _enemyAiFilter)
            {
                ref var enemyBehaviorAiComponent = ref enemyBehaviorAiPool.Get(entity);
                var gameObjectRefComponent = gameObjectRefPool.Get(entity);
                var atackParametersComponent = atackParametersPool.Get(entity);

                Vector3 entityPosition = gameObjectRefComponent.GameObject.transform.position;

                switch (enemyBehaviorAiComponent.CurrentState)
                {
                    case EnemyBehaviorAiComponent.State.SearchForTarget:
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

                            // Case: Set closest hero as target
                            var closestTarget = heroPositions.OrderBy(pair => (entityPosition - pair.Value).sqrMagnitude).First();

                            enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.MoveToTarget;
                            ref var targetComponent = ref targetPool.Add(entity);
                            targetComponent.TargetEntity = world.PackEntity(closestTarget.Key);

                            break;
                        }

                    case EnemyBehaviorAiComponent.State.MoveToTarget:
                        {
                            // Case: No target
                            if (!targetPool.Has(entity))
                            {
                                enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SearchForTarget;

                                break;
                            }

                            // Case: Target is not alive
                            ref var targetComponent = ref targetPool.Get(entity);
                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                // Target is missing, search for new one
                                enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SearchForTarget;
                                targetPool.Del(entity);

                                break;
                            }

                            ref var navigationComponent = ref navigationPool.Get(entity);

                            // Case: Too far from target, keep moving
                            Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;
                            if ((targetEntityPosition - entityPosition).magnitude > atackParametersComponent.Range)
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

                            // Case: Reached target point
                            enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.AttackTarget;

                            break;
                        }

                    case EnemyBehaviorAiComponent.State.AttackTarget:
                        {
                            // Case: Attack is happening
                            if (attackFlagPool.Has(entity))
                            {
                                break;
                            }

                            // Case: No target
                            if (!targetPool.Has(entity))
                            {
                                enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SearchForTarget;

                                break;
                            }

                            // Case: Target is not alive
                            ref var targetComponent = ref targetPool.Get(entity);
                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SearchForTarget;
                                targetPool.Del(entity);

                                break;
                            }

                            // Case: Target is too far
                            Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;
                            if ((entityPosition - targetEntityPosition).magnitude > atackParametersComponent.Range)
                            {
                                enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.MoveToTarget;

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

                            // Case: Start attack
                            attackRequestPool.Add(entity);

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
