using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Assets.Code.Ecs.Systems
{
    internal class ProjectileHitSystem : IEcsRunSystem
    {
        public const float HitThreshold = 0.1f;

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
            var damagePool = world.GetPool<DamageComponent>();
            var destroyRequestPool = world.GetPool<DestroyRequest>();

            foreach (var entity in _filter)
            {
                var projectileParametersComponent = projectileParametersPool.Get(entity);
                var targetComponent = targetPool.Get(entity);
                var gameObjectComponent = gameObjectRefPool.Get(entity);

                Vector3 entityPosition = gameObjectComponent.GameObject.transform.position;

                int targetEntity;
                if (!targetComponent.TargetEntity.Unpack(world, out targetEntity))
                {
                    // Target is not alive
                    destroyRequestPool.Add(entity);
                    continue;
                }

                Vector3 targetEntityPosition = gameObjectRefPool.Get(targetEntity).GameObject.transform.position;
                if ((entityPosition - targetEntityPosition).magnitude > HitThreshold)
                {
                    // Target is too far
                    continue;
                }

                // Hit target
                {
                    var damageEntity = world.NewEntity();

                    ref var damageTargetComponent = ref targetPool.Add(damageEntity);
                    damageTargetComponent.TargetEntity = targetComponent.TargetEntity;

                    ref var damageComponent = ref damagePool.Add(damageEntity);
                    damageComponent.Damage = projectileParametersComponent.Damage;
                }
                // Destroy projectile entity
                destroyRequestPool.Add(entity);
            }
        }
    }
}
