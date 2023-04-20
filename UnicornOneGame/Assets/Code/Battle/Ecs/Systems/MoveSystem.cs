using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class MoveSystem : IEcsRunSystem
    {
        private const float MovementSpeed = 5.0f;

        private readonly EcsCustomInject<ITimeService> _timeService;

        private EcsFilter _filter;
        private float DeltaSpeed => MovementSpeed * _timeService.Value.Delta;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<GameObjectUnityRefComponent>()
                    .Inc<MoveTargetComponent>()
                    .End();
            }

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            var moveTargetComponentPool = world.GetPool<MoveTargetComponent>();

            foreach (var entity in _filter)
            {
                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);
                var moveTargetComponent = moveTargetComponentPool.Get(entity);

                // Move object
                gameObjectUnityRefComponent.GameObject.transform.position =
                    Vector3.MoveTowards(
                        gameObjectUnityRefComponent.GameObject.transform.position,
                        moveTargetComponent.Position,
                        DeltaSpeed
                        );

                // Remove moveTarget if target is acquired
                if (gameObjectUnityRefComponent.GameObject.transform.position == moveTargetComponent.Position)
                {
                    moveTargetComponentPool.Del(entity);
                }
            }
        }
    }
}
