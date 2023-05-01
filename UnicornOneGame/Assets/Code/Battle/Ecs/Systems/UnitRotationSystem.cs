using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class UnitRotationSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<ITilemapService> _tilemapService;

        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<UnitFlag>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            var targetPositionMoveComponentPool = world.GetPool<TargetPositionMoveComponent>();
            var targetEntityComponentPool = world.GetPool<TargetEntityComponent>();
            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();

            foreach (var entity in _filter)
            {
                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);

                if (targetPositionMoveComponentPool.Has(entity))
                {
                    var targetPositionMoveComponent = targetPositionMoveComponentPool.Get(entity);
                    Vector3 targetDirection =
                        (targetPositionMoveComponent.Position - gameObjectUnityRefComponent.GameObject.transform.position).normalized;

                    gameObjectUnityRefComponent.GameObject.transform.forward = targetDirection;
                    //Vector3.RotateTowards(gameObjectUnityRefComponent.GameObject.transform.forward, targetDirection, 10.0f, 10.0f);

                    continue;
                }

                if (targetEntityComponentPool.Has(entity))
                {
                    var targetEntityComponent = targetEntityComponentPool.Get(entity);

                    if (targetEntityComponent.PackedEntity.Unpack(world, out int targetEntity))
                    {
                        var targetTilePositionComponent = tilePositionComponentPool.Get(targetEntity);
                        Vector3 targetPosition = targetTilePositionComponent.Position.ToWorldCoordsXZ(_tilemapService.Value.HexParams);
                        Vector3 targetDirection =
                            (targetPosition - gameObjectUnityRefComponent.GameObject.transform.position).normalized;

                        gameObjectUnityRefComponent.GameObject.transform.forward = targetDirection;
                        //Vector3.RotateTowards(gameObjectUnityRefComponent.GameObject.transform.forward, targetDirection, 10.0f, 10.0f);

                        continue;
                    }
                }
            }
        }
    }
}
