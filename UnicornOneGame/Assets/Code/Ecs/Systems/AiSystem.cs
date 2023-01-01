using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Components.AI;
using UnicornOne.Ecs.Components.Flags;
using UnicornOne.Ecs.Components.Refs;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class AiSystem : IEcsRunSystem
    {
        private EcsFilter _meleeHeroFilter;
        private EcsFilter _enemyFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_meleeHeroFilter == null)
            {
                _meleeHeroFilter = world
                    .Filter<HeroFlag>()
                    .Inc<MeleeFighterBehaviorAiComponent>()
                    .Inc<AtackParametersComponent>()
                    .Inc<NavigationComponent>()
                    .Inc<GameObjectRefComponent>()
                    .End();
            }

            var meleeFighterBehaviorAiPool = world.GetPool<MeleeFighterBehaviorAiComponent>();
            var atackParametersPool = world.GetPool<AtackParametersComponent>();
            var navigationPool = world.GetPool<NavigationComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectRefComponent>();
            var targetPool = world.GetPool<TargetComponent>();
            var attackRequestPool = world.GetPool<AttackRequest>();
            var attackFlagPool = world.GetPool<AttackFlag>();
            var attackRechargePool = world.GetPool<AttackRechargeComponent>();
            var standPool = world.GetPool<StandFlag>();

            Dictionary<int, Vector3> enemyPositions = null;

            foreach (var entity in _meleeHeroFilter)
            {
                ref var meleeFighterBehaviorAiComponent = ref meleeFighterBehaviorAiPool.Get(entity);
                ref var atackParametersComponent = ref atackParametersPool.Get(entity);
                ref var navigationComponent = ref navigationPool.Get(entity);
                ref var gameObjectRefComponent = ref gameObjectRefPool.Get(entity);

                Vector3 entityPosition = gameObjectRefComponent.GameObject.transform.position;

                switch (meleeFighterBehaviorAiComponent.CurrentState)
                {
                    case MeleeFighterBehaviorAiComponent.State.SearchForTarget:
                        {
                            if (enemyPositions == null)
                            {
                                enemyPositions = GetEnemyPositions(world);
                            }

                            // Case: No enemies
                            if (enemyPositions.Count == 0)
                            {
                                continue;
                            }

                            // Case: Set the closest enemy as target
                            meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.MoveToTarget;

                            var closestTarget = enemyPositions.OrderBy(pair => (entityPosition - pair.Value).sqrMagnitude).First();
                            ref var targetComponent = ref targetPool.Add(entity);
                            targetComponent.TargetEntity = world.PackEntity(closestTarget.Key);

                            break;
                        }

                    case MeleeFighterBehaviorAiComponent.State.MoveToTarget:
                        {
                            // Case: No target
                            if (!targetPool.Has(entity))
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.SearchForTarget;
                                break;
                            }

                            // Case: Target is not alive
                            ref var targetComponent = ref targetPool.Get(entity);
                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                // Target is missing, search for new one
                                meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.SearchForTarget;
                                targetPool.Del(entity);
                            }

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
                            meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.AttackTarget;

                            break;
                        }

                    case MeleeFighterBehaviorAiComponent.State.AttackTarget:
                        {
                            // Case: Attack is happening
                            if (attackFlagPool.Has(entity))
                            {
                                break;
                            }

                            // Case: No target
                            if (!targetPool.Has(entity))
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.SearchForTarget;

                                break;
                            }

                            // Case: Target is not alive
                            ref var targetComponent = ref targetPool.Get(entity);
                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.SearchForTarget;
                                targetPool.Del(entity);

                                break;
                            }

                            // Case: Target is too far
                            Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;
                            if ((entityPosition - targetEntityPosition).magnitude > atackParametersComponent.Range)
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.MoveToTarget;

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

        private Dictionary<int, Vector3> GetEnemyPositions(EcsWorld world)
        {
            if (_enemyFilter == null)
            {
                _enemyFilter = world
                    .Filter<EnemyFlag>()
                    .Inc<GameObjectRefComponent>()
                    .End();
            }

            Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>();

            var gameObjectRefPool = world.GetPool<GameObjectRefComponent>();

            foreach (var entity in _enemyFilter)
            {
                ref var gameObjectRefComponent = ref gameObjectRefPool.Get(entity);

                positions.Add(entity, gameObjectRefComponent.GameObject.transform.position);
            }

            return positions;
        }
    }
}
