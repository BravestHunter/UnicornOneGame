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
                    .Inc<MeleeAtackParametersComponent>()
                    .Inc<NavigationComponent>()
                    .Inc<GameObjectRefComponent>()
                    .End();
            }

            var meleeFighterBehaviorAiPool = world.GetPool<MeleeFighterBehaviorAiComponent>();
            var meleeAtackParametersPool = world.GetPool<MeleeAtackParametersComponent>();
            var navigationPool = world.GetPool<NavigationComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectRefComponent>();

            var targetPool = world.GetPool<TargetComponent>();
            var attackRequestPool = world.GetPool<AttackRequest>();
            var attackFlagPool = world.GetPool<AttackFlag>();

            Dictionary<int, Vector3> enemyPositions = null;

            foreach (var entity in _meleeHeroFilter)
            {
                ref var meleeFighterBehaviorAiComponent = ref meleeFighterBehaviorAiPool.Get(entity);
                ref var meleeAtackParametersComponent = ref meleeAtackParametersPool.Get(entity);
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

                            if (enemyPositions.Count == 0)
                            {
                                continue;
                            }

                            var closestTarget = enemyPositions.OrderBy(pair => (entityPosition - pair.Value).sqrMagnitude).First();

                            // Update components
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.MoveToTarget;

                                navigationComponent.DestionationPosition = closestTarget.Value;

                                ref var targetComponent = ref targetPool.Add(entity);
                                targetComponent.TargetEntity = world.PackEntity(closestTarget.Key);
                            }

                            break;
                        }

                    case MeleeFighterBehaviorAiComponent.State.MoveToTarget:
                        {
                            if (!targetPool.Has(entity))
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.SearchForTarget;
                                break;
                            }

                            ref var targetComponent = ref targetPool.Get(entity);

                            int targetEntity;
                            if (targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;

                                if ((targetEntityPosition - entityPosition).magnitude > meleeAtackParametersComponent.Range)
                                {
                                    // Still moving to target
                                    continue;
                                }
                            }
                            else
                            {
                                // Target is missing, search for new one
                                meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.SearchForTarget;
                                targetPool.Del(entity);
                            }

                            // Update components
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.AttackTarget;

                                navigationComponent.DestionationPosition = entityPosition;
                            }

                            break;
                        }

                    case MeleeFighterBehaviorAiComponent.State.AttackTarget:
                        {
                            if (attackFlagPool.Has(entity)) // Attack is happening
                            {
                                break;
                            }

                            if (!targetPool.Has(entity))
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.SearchForTarget;
                                break;
                            }

                            ref var targetComponent = ref targetPool.Get(entity);

                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.SearchForTarget;
                                targetPool.Del(entity);

                                break;
                            }

                            Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;
                            if ((entityPosition - targetEntityPosition).magnitude > meleeAtackParametersComponent.Range)
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.MoveToTarget;

                                break;
                            }

                            if (Time.timeSinceLevelLoad - meleeAtackParametersComponent.LastAttackTime >= 1.0f)
                            {
                                meleeAtackParametersComponent.LastAttackTime = Time.timeSinceLevelLoad;

                                attackRequestPool.Add(entity);
                            }

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
