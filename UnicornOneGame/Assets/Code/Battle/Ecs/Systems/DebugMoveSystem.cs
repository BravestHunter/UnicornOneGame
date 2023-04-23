using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Core.Geometry;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    public class DebugMoveSystem : IEcsRunSystem
    {
        private static readonly Vector3 GroundLineOffset = Vector3.up * 0.02f;

        private readonly EcsCustomInject<ITilemapService> _tilemapService;

        private EcsFilter _movepathFilter;
        private EcsFilter _destinationTileFilter;
        private EcsFilter _pathToDestinationFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            VisualizeMovepath(world);
            VisualizeDestinationTile(world);
            VisualizePathToDestination(world);
        }

        private void VisualizeMovepath(EcsWorld world)
        {
            if (_movepathFilter == null)
            {
                _movepathFilter = world
                    .Filter<GameObjectUnityRefComponent>()
                    .Inc<TargetPositionMoveComponent>()
                    .End();
            }

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            var targetPositionMoveComponentPool = world.GetPool<TargetPositionMoveComponent>();

            foreach (var entity in _movepathFilter)
            {
                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);
                var targetPositionMoveComponent = targetPositionMoveComponentPool.Get(entity);

                Debug.DrawLine(
                    gameObjectUnityRefComponent.GameObject.transform.position + GroundLineOffset,
                    targetPositionMoveComponent.Position + GroundLineOffset,
                    Color.red
                );
            }
        }

        private void VisualizeDestinationTile(EcsWorld world)
        {
            if (_destinationTileFilter == null)
            {
                _destinationTileFilter = world
                    .Filter<DestinationTileComponent>()
                    .End();
            }

            var destinationTileMoveComponentPool = world.GetPool<DestinationTileComponent>();

            foreach (var entity in _destinationTileFilter)
            {
                var destinationTileMoveComponent = destinationTileMoveComponentPool.Get(entity);

                DebugExtension.DebugPoint(
                    destinationTileMoveComponent.Position.ToWorldCoordsXZ(_tilemapService.Value.HexParams) + GroundLineOffset, Color.yellow
                );
            }
        }

        private void VisualizePathToDestination(EcsWorld world)
        {
            if (_pathToDestinationFilter == null)
            {
                _pathToDestinationFilter = world
                    .Filter<DestinationTileComponent>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            var destinationTileMoveComponentPool = world.GetPool<DestinationTileComponent>();
            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();

            foreach (var entity in _pathToDestinationFilter)
            {
                var destinationTileMoveComponent = destinationTileMoveComponentPool.Get(entity);
                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);

                Debug.DrawLine(
                    destinationTileMoveComponent.Position.ToWorldCoordsXZ(_tilemapService.Value.HexParams),
                    gameObjectUnityRefComponent.GameObject.transform.position,
                    Color.green
                    );
            }
        }
    }
}
