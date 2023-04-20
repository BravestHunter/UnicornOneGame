using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    public class TilepathMoveSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<ITilemapService> _tilemapService;

        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<TargetTileMoveComponent>()
                    .Exc<TargetPositionMoveComponent>()
                    .End();
            }

            var targetTileMoveComponentPool = world.GetPool<TargetTileMoveComponent>();
            var targetPositionMoveComponentPool = world.GetPool<TargetPositionMoveComponent>();

            foreach (var entity in _filter)
            {
                var targetTileMoveComponent = targetTileMoveComponentPool.Get(entity);
                ref var targetPositionMoveComponent = ref targetPositionMoveComponentPool.Add(entity);

                targetPositionMoveComponent.Position =
                    targetTileMoveComponent.Coords.ToWorldCoordsXZ(_tilemapService.Value.HexParams);

                targetTileMoveComponentPool.Del(entity);
            }
        }
    }
}
