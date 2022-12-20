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
        private EcsFilter _aiFilter;
        private EcsFilter _heroObjectsFilter;
        private EcsFilter _enemyObjectsFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_aiFilter == null)
            {
                _aiFilter = world
                    .Filter<AiBehaviorComponent>()
                    .Inc<GameObjectRefComponent>()
                    .Inc<NavigationAgentRefComponent>()
                    .Inc<MoveParametersComponent>()
                    .End();
            }

            if (_heroObjectsFilter == null)
            {
                _heroObjectsFilter = world
                    .Filter<GameObjectRefComponent>()
                    .Inc<HeroFlag>()
                    .End();
            }

            if (_enemyObjectsFilter == null)
            {
                _enemyObjectsFilter = world
                    .Filter<GameObjectRefComponent>()
                    .Inc<EnemyFlag>()
                    .End();
            }

            var aiBehaviorPool = world.GetPool<AiBehaviorComponent>();
            var targetAiPool = world.GetPool<TargetAiComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectRefComponent>();
            var navigationAgentRefPool = world.GetPool<NavigationAgentRefComponent>();
            var heroFlagPool = world.GetPool<HeroFlag>();
            var DestroyRequestPool = world.GetPool<DestroyRequest>();
            var moveParametersPool = world.GetPool<MoveParametersComponent>();

            Dictionary<int, Vector3> heroPositions = null;
            Dictionary<int, Vector3> enemyPositions = null;

            foreach (var entity in _aiFilter)
            {
                ref var aiBehaviorComponent = ref aiBehaviorPool.Get(entity);
                ref var gameObjectRefComponent = ref gameObjectRefPool.Get(entity);

                Vector3 position = gameObjectRefComponent.GameObject.transform.position;
                bool isHero = heroFlagPool.Has(entity);

                switch (aiBehaviorComponent.State)
                {
                    case AiBehaviorComponent.HeroAiState.SearchingForTarget:
                        {
                            Dictionary<int, Vector3> targets;
                            if (isHero)
                            {
                                if (enemyPositions == null)
                                {
                                    enemyPositions = CollectPositions(world, _enemyObjectsFilter);
                                }
                                targets = enemyPositions;
                            }
                            else
                            {
                                if (heroPositions == null)
                                {
                                    heroPositions = CollectPositions(world, _heroObjectsFilter);
                                }
                                targets = heroPositions;
                            }

                            if (targets.Count == 0)
                            {
                                continue;
                            }

                            var closestTarget = targets.OrderBy(pair => (position - pair.Value).sqrMagnitude).First();

                            ref var targetAiComponent = ref targetAiPool.Add(entity);
                            targetAiComponent.Target = world.PackEntity(closestTarget.Key);

                            ref var moveParametersComponent = ref moveParametersPool.Get(entity);

                            aiBehaviorComponent.State = AiBehaviorComponent.HeroAiState.WalkingToTarget;
                            ref var navigationAgentRefComponent = ref navigationAgentRefPool.Get(entity);
                            navigationAgentRefComponent.Agent.speed = moveParametersComponent.Speed;
                            navigationAgentRefComponent.Agent.destination = closestTarget.Value;
                        }
                        break;

                    case AiBehaviorComponent.HeroAiState.WalkingToTarget:
                        {
                            ref var targetAiComponent = ref targetAiPool.Get(entity);

                            int targetEntity;
                            if (targetAiComponent.Target.Unpack(world, out targetEntity))
                            {
                                Vector3 targetGameObjectPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;

                                if ((targetGameObjectPosition - position).sqrMagnitude <= 0.1f)
                                {
                                    DestroyRequestPool.Add(targetEntity);
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            aiBehaviorComponent.State = AiBehaviorComponent.HeroAiState.SearchingForTarget;
                            
                            targetAiPool.Del(entity);

                            ref var moveParametersComponent = ref moveParametersPool.Get(entity);

                            ref var navigationAgentRefComponent = ref navigationAgentRefPool.Get(entity);
                            navigationAgentRefComponent.Agent.speed = moveParametersComponent.Speed;
                            navigationAgentRefComponent.Agent.destination = position;
                        }
                        break;
                }
            }
        }

        private Dictionary<int, Vector3> CollectPositions(EcsWorld world, EcsFilter filter)
        {
            Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>();

            var gameObjectRefPool = world.GetPool<GameObjectRefComponent>();

            foreach (var entity in filter)
            {
                ref var gameObjectRefComponent = ref gameObjectRefPool.Get(entity);

                positions.Add(entity, gameObjectRefComponent.GameObject.transform.position);
            }

            return positions;
        }
    }
}
