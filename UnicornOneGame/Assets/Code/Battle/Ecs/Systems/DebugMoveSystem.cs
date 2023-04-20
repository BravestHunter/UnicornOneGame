using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Components.Flags;
using UnicornOne.Battle.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    public class DebugMoveSystem : IEcsRunSystem
    {
        private static readonly Vector3 GroundLineOffset = Vector3.up * 0.02f;

        private readonly EcsCustomInject<ITilemapService> _tilemapService;

        private EcsFilter _movepathFilter;
        private EcsFilter _tilepathFilter;
        private EcsFilter _destinationTileFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            VisualizeMovepath(world);
            VisualizeTilepath(world);
            VisualizeDestinationTile(world);
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

        private void VisualizeTilepath(EcsWorld world)
        {
            if (_tilepathFilter == null)
            {
                _tilepathFilter = world
                    .Filter<TilepathMoveComponent>()
                    .End();
            }

            var tilepathMoveComponentPool = world.GetPool<TilepathMoveComponent>();

            foreach (var entity in _tilepathFilter)
            {
                var tilepathMoveComponent = tilepathMoveComponentPool.Get(entity);

                for (int i = 1; i < tilepathMoveComponent.Path.Count; i++)
                {
                    Debug.DrawLine(
                        tilepathMoveComponent.Path[i - 1].ToWorldCoordsXZ(_tilemapService.Value.HexParams) + GroundLineOffset,
                        tilepathMoveComponent.Path[i].ToWorldCoordsXZ(_tilemapService.Value.HexParams) + GroundLineOffset,
                        Color.blue
                    );
                }
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
    }
}
