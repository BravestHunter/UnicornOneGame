using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class DebugTargetSystem : IEcsRunSystem
    {
        private static readonly Color Color = Color.cyan;

        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<TargetEntityComponent>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            var targetEntityComponentPool = world.GetPool<TargetEntityComponent>();
            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();

            foreach (var entity in _filter)
            {
                var targetEntityComponent = targetEntityComponentPool.Get(entity);
                if (targetEntityComponent.PackedEntity.Unpack(world, out int targetEntity))
                {
                    var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);
                    var targetGameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(targetEntity);

                    Debug.DrawLine(
                        gameObjectUnityRefComponent.GameObject.transform.position, 
                        targetGameObjectUnityRefComponent.GameObject.transform.position,
                        Color
                    );
                    DebugExtension.DebugArrow(
                        gameObjectUnityRefComponent.GameObject.transform.position,
                        (targetGameObjectUnityRefComponent.GameObject.transform.position - gameObjectUnityRefComponent.GameObject.transform.position).normalized,
                        Color
                    );
                    DebugExtension.DebugPoint(gameObjectUnityRefComponent.GameObject.transform.position, Color, 0.5f);
                }
            }
        }
    }
}
