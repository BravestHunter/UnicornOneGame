using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnicornOne.Ecs.Systems
{
    internal class SquadAiSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _squadAiFilter;
        private EcsFilter _heroFilter;
        private EcsFilter _enemyFilter;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var squadAiEntity = world.NewEntity();

            var squadAiPool = world.GetPool<SquadAiFlag>();
            squadAiPool.Add(squadAiEntity);

            var targetPool = world.GetPool<TargetComponent>();
            targetPool.Add(squadAiEntity);
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_squadAiFilter == null)
            {
                _squadAiFilter = world
                    .Filter<SquadAiFlag>()
                    .End();
            }

            var targetPool = world.GetPool<TargetComponent>();

            foreach (var entity in _squadAiFilter)
            {
                ref var targetComponent = ref targetPool.Get(entity);
                if (targetComponent.TargetEntity.Unpack(world, out int targetEntity))
                {
                    continue;
                }

                var heroPositions = GetHeroPositions(world);
                if (heroPositions.Count == 0)
                {
                    continue;
                }

                var enemyPositions = GetEnemyPositions(world);
                if (enemyPositions.Count == 0)
                {
                    continue;
                }

                var heroPositionAverage = heroPositions.Values.Aggregate(Vector3.zero, (s, v) => s + v) / heroPositions.Count;
                var closestTarget = enemyPositions.OrderBy(pair => (heroPositionAverage - pair.Value).sqrMagnitude).First();

                targetComponent.TargetEntity = world.PackEntity(closestTarget.Key);
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

            return GetObjectPositions(_heroFilter, world);
        }

        private Dictionary<int, Vector3> GetEnemyPositions(EcsWorld world)
        {
            if (_enemyFilter == null)
            {
                _enemyFilter = world
                    .Filter<EnemyFlag>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            return GetObjectPositions(_enemyFilter, world);
        }

        private static Dictionary<int, Vector3> GetObjectPositions(EcsFilter filter, EcsWorld world)
        {
            Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>();

            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();

            foreach (var entity in filter)
            {
                ref var gameObjectRefComponent = ref gameObjectRefPool.Get(entity);

                positions.Add(entity, gameObjectRefComponent.GameObject.transform.position);
            }

            return positions;
        }
    }
}
