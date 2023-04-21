using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Core.Utils;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class NavigationSystem : IEcsRunSystem
    {
        private static readonly HexPathFinder _pathFinder = new HexPathFinder();

        private EcsFilter _tilepathDeletionFilter;
        private EcsFilter _tilepathGenerationFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessInvalidTilepathDeletion(world);
            ProcessTilepathsGeneration(world);
        }

        private void ProcessInvalidTilepathDeletion(EcsWorld world)
        {
            if (_tilepathDeletionFilter == null)
            {
                _tilepathDeletionFilter = world
                    .Filter<DestinationTileComponent>()
                    .Inc<TilepathMoveComponent>()
                    .End();
            }

            var destinationTileComponentPool = world.GetPool<DestinationTileComponent>();
            var tilepathMoveComponentPool = world.GetPool<TilepathMoveComponent>();

            foreach (var entity in _tilepathDeletionFilter)
            {
                var tilepathMoveComponent = tilepathMoveComponentPool.Get(entity);

                if (tilepathMoveComponent.Path == null)
                {
                    tilepathMoveComponentPool.Del(entity);
                    continue;
                }

                var destinationTileComponent = destinationTileComponentPool.Get(entity);
                if (tilepathMoveComponent.Path.Count > 0 && destinationTileComponent.Position != tilepathMoveComponent.Path.First())
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
                    .Exc<TilepathMoveComponent>()
                    .End();
            }

            var destinationTileComponentPool = world.GetPool<DestinationTileComponent>();
            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            var tilepathMoveComponentPool = world.GetPool<TilepathMoveComponent>();

            foreach (var entity in _tilepathGenerationFilter)
            {
                var destinationTileComponent = destinationTileComponentPool.Get(entity);
                var tilePositionComponent = tilePositionComponentPool.Get(entity);

                if (tilePositionComponent.Position == destinationTileComponent.Position)
                {
                    destinationTileComponentPool.Del(entity);
                    continue;
                }

                ref var tilepathMoveComponent = ref tilepathMoveComponentPool.Add(entity);
                tilepathMoveComponent.Path = _pathFinder.FindPath(tilePositionComponent.Position, destinationTileComponent.Position);
            }
        }
    }
}
