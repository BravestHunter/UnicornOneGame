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

        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessTilepathsGeneration(world);
        }

        private void ProcessTilepathsGeneration(EcsWorld world)
        {
            if (_filter == null)
            {
                _filter = world
                    .Filter<DestinationTileComponent>()
                    .Inc<TilePositionComponent>()
                    .Exc<TargetTileMoveComponent>()
                    .End();
            }

            var destinationTileComponentPool = world.GetPool<DestinationTileComponent>();
            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            var targetTileMoveComponentPool = world.GetPool<TargetTileMoveComponent>();

            foreach (var entity in _filter)
            {
                var destinationTileComponent = destinationTileComponentPool.Get(entity);
                var tilePositionComponent = tilePositionComponentPool.Get(entity);

                if (tilePositionComponent.Position == destinationTileComponent.Position)
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

                _tilemapService.Value.Tilemap.Tiles[targetTileMoveComponent.Position].IsReserved = true;
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
