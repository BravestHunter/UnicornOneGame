using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.Models;
using UnicornOne.Core.Utils;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class NavigationSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<ITilemapService> _tilemapService;

        private static readonly HexPathFinder _pathFinder = new HexPathFinder();

        private EcsFilter _tilepathDeletionFilter;
        private EcsFilter _tilepathGenerationFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            //ProcessInvalidTilepathDeletion(world);
            ProcessTilepathsGeneration(world);
        }

        private void ProcessInvalidTilepathDeletion(EcsWorld world)
        {
            if (_tilepathDeletionFilter == null)
            {
                _tilepathDeletionFilter = world
                    .Filter<DestinationTileComponent>()
                    .Inc<TilePositionComponent>()
                    .Inc<TilepathMoveComponent>()
                    .Exc<TargetTileMoveComponent>()
                    .End();
            }

            var destinationTileComponentPool = world.GetPool<DestinationTileComponent>();
            var tilepathMoveComponentPool = world.GetPool<TilepathMoveComponent>();

            foreach (var entity in _tilepathDeletionFilter)
            {
                var tilepathMoveComponent = tilepathMoveComponentPool.Get(entity);

                if (tilepathMoveComponent.Path == null || tilepathMoveComponent.Path.Count == 0)
                {
                    tilepathMoveComponentPool.Del(entity);
                    continue;
                }

                var destinationTileComponent = destinationTileComponentPool.Get(entity);
                if (destinationTileComponent.Position != tilepathMoveComponent.Path.First())
                {
                    tilepathMoveComponentPool.Del(entity);
                    continue;
                }

                if (tilepathMoveComponent.Path.Any(tile => !IsTileAvailable(tile)))
                {
                    tilepathMoveComponentPool.Del(entity);
                    continue;
                }
            }
        }

        private void ProcessTilepathsGeneration(EcsWorld world)
        {
            if (_tilepathGenerationFilter == null)
            {
                _tilepathGenerationFilter = world
                    .Filter<DestinationTileComponent>()
                    .Inc<TilePositionComponent>()
                    //.Exc<TilepathMoveComponent>()
                    .Exc<TargetTileMoveComponent>()
                    .End();
            }

            var destinationTileComponentPool = world.GetPool<DestinationTileComponent>();
            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            var tilepathMoveComponentPool = world.GetPool<TilepathMoveComponent>();
            var targetTileMoveComponentPool = world.GetPool<TargetTileMoveComponent>();

            foreach (var entity in _tilepathGenerationFilter)
            {
                var destinationTileComponent = destinationTileComponentPool.Get(entity);
                var tilePositionComponent = tilePositionComponentPool.Get(entity);

                if (tilePositionComponent.Position == destinationTileComponent.Position ||
                    !IsTileAvailable(destinationTileComponent.Position))
                {
                    destinationTileComponentPool.Del(entity);
                    continue;
                }

                var path = _pathFinder.FindPath(tilePositionComponent.Position, destinationTileComponent.Position, IsTileAvailable);
                if (path == null)
                {
                    continue;
                }

                ref var targetTileMoveComponent = ref targetTileMoveComponentPool.Add(entity);
                targetTileMoveComponent.Position = path[path.Count - 2];

                //ref var tilepathMoveComponent = ref tilepathMoveComponentPool.Add(entity);
                //tilepathMoveComponent.Path = _pathFinder.FindPath(tilePositionComponent.Position, destinationTileComponent.Position, IsTileAvailable);
            }
        }

        private bool IsTileAvailable(HexCoords coords)
        {
            Tile tile;
            if (!_tilemapService.Value.Tilemap.Tiles.TryGetValue(coords, out tile))
            {
                return false;
            }

            return tile.IsAvailable;
        }
    }
}
