using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Core.Utils;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class RandomDestinationTileChooseSystem : IEcsRunSystem
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
                    .Inc<TilePositionComponent>()
                    .Inc<RangomDestinationTileChoseFlag>()
                    .Exc<DestinationTileComponent>()
                    .End();
            }

            var destinationTileComponentPool = world.GetPool<DestinationTileComponent>();

            foreach (var entity in _filter)
            {
                ref var destinationTileComponent = ref destinationTileComponentPool.Add(entity);

                destinationTileComponent.Position = _tilemapService.Value.GetRandomAvailablePosition();
            }
        }
    }
}
