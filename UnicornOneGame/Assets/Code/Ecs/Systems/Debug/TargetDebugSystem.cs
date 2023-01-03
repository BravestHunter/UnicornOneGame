using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class TargetDebugSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<TargetComponent>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            var targetPool = world.GetPool<TargetComponent>();
            var gameObjectUnityRefPool = world.GetPool<GameObjectUnityRefComponent>();

            foreach (var entity in _filter)
            {
                var targetComponent = targetPool.Get(entity);

                int targetEntity;
                if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                {
                    continue;
                }

                Vector3 from = gameObjectUnityRefPool.Get(entity).GameObject.transform.position;
                Vector3 to = gameObjectUnityRefPool.Get(targetEntity).GameObject.transform.position;

                Debug.DrawLine(from, to, Color.green);
            }
        }
    }
}
