using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class MoveSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<ITimeService> _timeService;

        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<GameObjectUnityRefComponent>()
                    .Inc<TargetPositionMoveComponent>()
                    .Inc<MovementComponent>()
                    .End();
            }

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            var targetPositionMoveComponentPool = world.GetPool<TargetPositionMoveComponent>();
            var movementComponentPool = world.GetPool<MovementComponent>();

            foreach (var entity in _filter)
            {
                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);
                var targetPositionMoveComponent = targetPositionMoveComponentPool.Get(entity);
                var movementComponent = movementComponentPool.Get(entity);

                // Move object
                gameObjectUnityRefComponent.GameObject.transform.position =
                    Vector3.MoveTowards(
                        gameObjectUnityRefComponent.GameObject.transform.position,
                        targetPositionMoveComponent.Position,
                        movementComponent.Speed * _timeService.Value.Delta
                    );

                // Remove moveTarget if target is acquired
                if (gameObjectUnityRefComponent.GameObject.transform.position == targetPositionMoveComponent.Position)
                {
                    targetPositionMoveComponentPool.Del(entity);
                }
            }
        }
    }
}
