using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class AnimationSystem : IEcsRunSystem
    {
        private EcsFilter _updateFilter;
        private EcsFilter _triggerFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessUpdate(world);
            ProcessTriggers(world);
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

        private void ProcessTriggers(EcsWorld world)
        {
            if (_triggerFilter == null)
            {
                _triggerFilter = world
                    .Filter<AnimatorUnityRefComponent>()
                    .Inc<AnimationTriggerComponent>()
                    .End();
            }

            var animatorUnityRefComponentPool = world.GetPool<AnimatorUnityRefComponent>();
            var animationTriggerComponentPool = world.GetPool<AnimationTriggerComponent>();

            foreach (var entity in _triggerFilter)
            {
                var animatorUnityRefComponent = animatorUnityRefComponentPool.Get(entity);
                var animationTriggerComponent = animationTriggerComponentPool.Get(entity);

                animatorUnityRefComponent.Animator.SetTrigger(animationTriggerComponent.Name);

                animationTriggerComponentPool.Del(entity);
            }
        }
    }
}
