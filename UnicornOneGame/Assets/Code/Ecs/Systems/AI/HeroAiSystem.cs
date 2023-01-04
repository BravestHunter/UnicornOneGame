using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class HeroAiSystem : IEcsRunSystem
    {
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
                    .Inc<AtackParametersComponent>()
                    .Inc<NavigationComponent>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            var heroBehaviorAiPool = world.GetPool<HeroBehaviorAiComponent>();
            var atackParametersPool = world.GetPool<AtackParametersComponent>();
            var navigationPool = world.GetPool<NavigationComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();
            var targetPool = world.GetPool<TargetComponent>();
            var attackRequestPool = world.GetPool<AttackRequest>();
            var attackFlagPool = world.GetPool<AttackFlag>();
            var attackRechargePool = world.GetPool<AttackRechargeComponent>();
            var standPool = world.GetPool<StandFlag>();

            foreach (var entity in _heroFilter)
            {
                ref var meleeFighterBehaviorAiComponent = ref heroBehaviorAiPool.Get(entity);
                ref var atackParametersComponent = ref atackParametersPool.Get(entity);
                ref var navigationComponent = ref navigationPool.Get(entity);
                ref var gameObjectRefComponent = ref gameObjectRefPool.Get(entity);

                Vector3 entityPosition = gameObjectRefComponent.GameObject.transform.position;

                switch (meleeFighterBehaviorAiComponent.CurrentState)
                {
                    case HeroBehaviorAiComponent.State.SearchForTarget:
                        {
                            // Case: Squad leader has no target
                            if (squadTargetComponent == null)
                            {
                                break;
                            }

                            // Case: Set target of squad leader
                            meleeFighterBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.MoveToTarget;

                            ref var targetComponent = ref targetPool.Add(entity);
                            targetComponent.TargetEntity = squadTargetComponent.Value.TargetEntity;

                            break;
                        }

                    case HeroBehaviorAiComponent.State.MoveToTarget:
                        {
                            // Case: No target
                            if (!targetPool.Has(entity))
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.SearchForTarget;

                                break;
                            }

                            // Case: Target is not alive
                            ref var targetComponent = ref targetPool.Get(entity);
                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                // Target is missing, search for new one
                                meleeFighterBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.SearchForTarget;
                                targetPool.Del(entity);

                                break;
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
                            meleeFighterBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.AttackTarget;

                            break;
                        }

                    case HeroBehaviorAiComponent.State.AttackTarget:
                        {
                            // Case: Attack is happening
                            if (attackFlagPool.Has(entity))
                            {
                                break;
                            }

                            // Case: No target
                            if (!targetPool.Has(entity))
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.SearchForTarget;

                                break;
                            }

                            // Case: Target is not alive
                            ref var targetComponent = ref targetPool.Get(entity);
                            int targetEntity;
                            if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.SearchForTarget;
                                targetPool.Del(entity);

                                break;
                            }

                            // Case: Target is too far
                            Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;
                            if ((entityPosition - targetEntityPosition).magnitude > atackParametersComponent.Range)
                            {
                                meleeFighterBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.MoveToTarget;

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
