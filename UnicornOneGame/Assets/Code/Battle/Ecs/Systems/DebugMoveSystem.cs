using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    public class DebugMoveSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<GameObjectUnityRefComponent>()
                    .Inc<TargetPositionMoveComponent>()
                    .End();
            }

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            var targetPositionMoveComponentPool = world.GetPool<TargetPositionMoveComponent>();

            foreach (var entity in _filter)
            {
                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);
                var targetPositionMoveComponent = targetPositionMoveComponentPool.Get(entity);

                Debug.DrawLine(
                    gameObjectUnityRefComponent.GameObject.transform.position,
                    targetPositionMoveComponent.Position,
                    Color.blue
                );
                DebugExtension.DebugPoint(targetPositionMoveComponent.Position, Color.green);
            }
        }
    }
}
