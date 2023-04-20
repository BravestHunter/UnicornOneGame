using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Core.Utils;
using UnityEngine;
using static log4net.Appender.ColoredConsoleAppender;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class RandomUnitMoveTargetSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        private HexCoords RandomHexCoords
        {
            get
            {
                int q = Random.Range(-6, 7);
                int r = Random.Range(-6, 7);
                return HexCoords.FromCube(q, r);
            }
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<UnitFlag>()
                    .Exc<TargetTileMoveComponent>()
                    .Exc<TargetPositionMoveComponent>()
                    .End();
            }

            var targetTileMoveComponentPool = world.GetPool<TargetTileMoveComponent>();

            foreach (var entity in _filter)
            {
                ref var targetTileMoveComponent = ref targetTileMoveComponentPool.Add(entity);

                targetTileMoveComponent.Coords = RandomHexCoords;
            }
        }
    }
}
