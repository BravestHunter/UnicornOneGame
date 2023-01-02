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
    internal class ProjectileMoveSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<ProjectileParametersComponent>()
                    .Inc<TargetComponent>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            var projectileParametersPool = world.GetPool<ProjectileParametersComponent>();
            var targetPool = world.GetPool<TargetComponent>();
            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();

            foreach (var entity in _filter)
            {
                var projectileParametersComponent = projectileParametersPool.Get(entity);
                var targetComponent = targetPool.Get(entity);
                var gameObjectComponent = gameObjectRefPool.Get(entity);

                int targetEntity;
                if (targetComponent.TargetEntity.Unpack(world, out targetEntity))
                {
                    Vector3 entityPosition = gameObjectComponent.GameObject.transform.position;
                    entityPosition.y = 0;
                    Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;
                    targetEntityPosition.y = 0;
                    Vector3 offsetToTarget = targetEntityPosition - entityPosition;

                    float moveDistance = Time.deltaTime * projectileParametersComponent.MoveSpeed;

                    if (offsetToTarget.magnitude <= moveDistance)
                    {
                        gameObjectComponent.GameObject.transform.position = targetEntityPosition;
                    }
                    else
                    {
                        gameObjectComponent.GameObject.transform.position += offsetToTarget.normalized * moveDistance;
                    }
                }
            }
        }
    }
}
