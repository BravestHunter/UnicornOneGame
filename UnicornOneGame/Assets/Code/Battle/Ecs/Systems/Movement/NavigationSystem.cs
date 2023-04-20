using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Components.Flags;
using UnicornOne.Core.Utils;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class NavigationSystem : IEcsRunSystem
    {
        private static readonly HexPathFinder _pathFinder = new HexPathFinder();

        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<DestinationTileComponent>()
                    .Inc<TilePositionComponent>()
                    .Exc<TilepathMoveComponent>()
                    .End();
            }

            var destinationTileComponentPool = world.GetPool<DestinationTileComponent>();
            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            var tilepathMoveComponentPool = world.GetPool<TilepathMoveComponent>();

            foreach (var entity in _filter)
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
