using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    public class AnimationSystem : IEcsRunSystem
    {
        private EcsFilter _updateFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessUpdate(world);
        }

        private void ProcessUpdate(EcsWorld world)
        {
            if (_updateFilter == null)
            {
                _updateFilter = world
                    .Filter<AnimatorUnityRefComponent>()
                    .Inc<MovementComponent>()
                    .End();
            }

            var animatorUnityRefComponentPool = world.GetPool<AnimatorUnityRefComponent>();
            var movementComponentPool = world.GetPool<MovementComponent>();
            var destinationTileComponentPool = world.GetPool<DestinationTileComponent>();

            foreach (var entity in _updateFilter)
            {
                var animatorUnityRefComponent = animatorUnityRefComponentPool.Get(entity);

                animatorUnityRefComponent.Animator.SetBool("Moving", destinationTileComponentPool.Has(entity));

                var movementComponent = movementComponentPool.Get(entity);
                animatorUnityRefComponent.Animator.SetFloat("VelocityZ", movementComponent.Speed);
            }
        }
    }
}
