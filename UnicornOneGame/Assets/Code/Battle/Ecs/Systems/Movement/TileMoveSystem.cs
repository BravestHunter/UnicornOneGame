using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems.Movement
{
    internal class TileMoveSystem : IEcsRunSystem
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
                    .Inc<GameObjectUnityRefComponent>()
                    .Inc<TilePositionComponent>()
                    .Exc<TargetPositionMoveComponent>()
                    .End();
            }

            var targetTileMoveComponentPool = world.GetPool<TargetTileMoveComponent>();
            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            var targetPositionMoveComponentPool = world.GetPool<TargetPositionMoveComponent>();

            foreach (var entity in _filter)
            {
                var targetTileMoveComponent = targetTileMoveComponentPool.Get(entity);
                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);
                ref var tilePositionComponent = ref tilePositionComponentPool.Get(entity);

                Vector3 targetWorldPosition = targetTileMoveComponent.Position.ToWorldCoordsXZ(_tilemapService.Value.HexParams);

                if (targetWorldPosition == gameObjectUnityRefComponent.GameObject.transform.position)
                {
                    // So we already reached target cell
                    _tilemapService.Value.Tilemap.Tiles[tilePositionComponent.Position].IsReserved = false;
                    tilePositionComponent.Position = targetTileMoveComponent.Position;
                    targetTileMoveComponentPool.Del(entity);
                    continue;
                }

                if (targetTileMoveComponent.Position.DistanceTo(tilePositionComponent.Position) > 1)
                {
                    // Something wrong with target cell,
                    // it should only be one of neighbor cells
                    targetTileMoveComponentPool.Del(entity);
                    continue;
                }

                ref var targetPositionMoveComponent = ref targetPositionMoveComponentPool.Add(entity);
                targetPositionMoveComponent.Position = targetWorldPosition;
                _tilemapService.Value.Tilemap.Tiles[targetTileMoveComponent.Position].IsReserved = true;
            }
        }
    }
}
