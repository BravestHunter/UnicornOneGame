using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    public class TilepathMoveSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<TilepathMoveComponent>()
                    .Inc<TilePositionComponent>()
                    .Exc<TargetTileMoveComponent>()
                    .End();
            }

            var tilepathMoveComponentPool = world.GetPool<TilepathMoveComponent>();
            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            var targetTileMoveComponentPool = world.GetPool<TargetTileMoveComponent>();

            foreach (var entity in _filter)
            {
                var tilepathMoveComponent = tilepathMoveComponentPool.Get(entity);
                if (tilepathMoveComponent.Path == null || tilepathMoveComponent.Path.Count == 0)
                {
                    // Invalid path
                    tilepathMoveComponentPool.Del(entity);
                    continue;
                }

                var tilePositionComponent = tilePositionComponentPool.Get(entity);
                if (tilePositionComponent.Position.DistanceTo(tilepathMoveComponent.Path.Last()) > 1)
                {
                    // Something wrong with next tile in path,
                    // it should be neghibor cell to current one
                    tilepathMoveComponentPool.Del(entity);
                    continue;
                }

                ref var targetTileMoveComponent = ref targetTileMoveComponentPool.Add(entity);
                targetTileMoveComponent.Position = tilepathMoveComponent.Path.Last();
                tilepathMoveComponent.Path.RemoveAt(tilepathMoveComponent.Path.Count - 1);
            }
        }
    }
}
