using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class RandomUnitMoveTargetSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        private Vector3 RandomTargetPosition
        {
            get
            {
                Vector2 random = Random.insideUnitCircle * 15.0f;
                return new Vector3(random.x, 0.0f, random.y);
            }
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<UnitFlag>()
                    .Exc<MoveTargetComponent>()
                    .End();
            }

            var moveTargetComponentPool = world.GetPool<MoveTargetComponent>();

            foreach (var entity in _filter)
            {
                ref var moveTargetComponent = ref moveTargetComponentPool.Add(entity);

                moveTargetComponent.Position = RandomTargetPosition;
            }
        }
    }
}
