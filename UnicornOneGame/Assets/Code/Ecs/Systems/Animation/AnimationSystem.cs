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
    internal class AnimationSystem : IEcsRunSystem
    {
        private EcsFilter _animatedEntityFilter;
        private EcsFilter _animatorTriggerRequestFilter;
        private EcsFilter _animationEventFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessAnimatorStateUpdates(world);
            ProcessAnimationTriggerRequests(world);
            ProcessAnimationEvents(world);
        }

        private void ProcessAnimatorStateUpdates(EcsWorld world)
        {
            if (_animatedEntityFilter == null)
            {
                _animatedEntityFilter = world
                    .Filter<AnimatorUnityRefComponent>()
                    .End();
            }

            var animatorRefPool = world.GetPool<AnimatorUnityRefComponent>();
            var navigationPool = world.GetPool<NavigationComponent>();
            var standPool = world.GetPool<StandFlag>();
            var attackAnimationFlagPool = world.GetPool<AttackAnimationFlag>();

            foreach (var entity in _animatedEntityFilter)
            {
                var animatorRefComponent = animatorRefPool.Get(entity);

                // Update state
                animatorRefComponent.Animator.SetFloat("Animation Speed", 1.0f); // Always 1.0 by now
                animatorRefComponent.Animator.SetBool("Blocking", false);
                animatorRefComponent.Animator.SetBool("Targeting", false);
                animatorRefComponent.Animator.SetBool("Weapons", true);

                if (!standPool.Has(entity) && navigationPool.Has(entity))
                {
                    var navigationComponent = navigationPool.Get(entity);

                    animatorRefComponent.Animator.SetBool("Moving", true);
                    animatorRefComponent.Animator.SetFloat("Velocity", navigationComponent.MovementSpeed);
                    animatorRefComponent.Animator.SetFloat("Velocity X", navigationComponent.MovementSpeed);
                    animatorRefComponent.Animator.SetFloat("Velocity Z", navigationComponent.MovementSpeed);
                }
                else
                {
                    animatorRefComponent.Animator.SetBool("Moving", false);
                    animatorRefComponent.Animator.SetFloat("Velocity", 0.0f);
                    animatorRefComponent.Animator.SetFloat("Velocity X", 0.0f);
                    animatorRefComponent.Animator.SetFloat("Velocity Z", 0.0f);
                }

                // On animation finished
                if (attackAnimationFlagPool.Has(entity))
                {
                    var currentState = animatorRefComponent.Animator.GetCurrentAnimatorStateInfo(0);
                    var nextState = animatorRefComponent.Animator.GetNextAnimatorStateInfo(0);

                    // We have to check next state, because animation may start not immediately
                    if (nextState.IsName("Attack1")) // Attack animation is going to start
                    {
                        continue;
                    }

                    if (currentState.IsName("Idle") || currentState.IsName("Movement Blend")) // Attack is finished
                    {
                        attackAnimationFlagPool.Del(entity);
                    }
                }
            }
        }

        private void ProcessAnimationTriggerRequests(EcsWorld world)
        {
            if (_animatorTriggerRequestFilter == null)
            {
                _animatorTriggerRequestFilter = world
                    .Filter<AnimatorTriggerRequest>()
                    .Inc<AnimatorUnityRefComponent>()
                    .End();
            }

            var animatorTriggerRequestPool = world.GetPool<AnimatorTriggerRequest>();
            var animatorRefPool = world.GetPool<AnimatorUnityRefComponent>();
            var attackAnimationFlagPool = world.GetPool<AttackAnimationFlag>();

            foreach (var entity in _animatorTriggerRequestFilter)
            {
                var animatorTriggerRequest = animatorTriggerRequestPool.Get(entity);
                var animatorRefComponent = animatorRefPool.Get(entity);

                animatorRefComponent.Animator.SetTrigger(animatorTriggerRequest.Name);

                attackAnimationFlagPool.Add(entity);
                animatorTriggerRequestPool.Del(entity);
            }
        }

        private void ProcessAnimationEvents(EcsWorld world)
        {
            if (_animationEventFilter == null)
            {
                _animationEventFilter = world
                    .Filter<AnimationEventHandlerUnityRefComponent>()
                    .End();
            }

            var animationEventHandlerRefPool = world.GetPool<AnimationEventHandlerUnityRefComponent>();
            var hitRequestPool = world.GetPool<HitRequest>();

            foreach (var entity in _animationEventFilter)
            {
                ref var animationEventHandlerRefComponent = ref animationEventHandlerRefPool.Get(entity);

                if (animationEventHandlerRefComponent.AnimationEventHandler.HitFlag)
                {
                    if (!hitRequestPool.Has(entity))
                    {
                        hitRequestPool.Add(entity);
                    }
                }

                animationEventHandlerRefComponent.AnimationEventHandler.Clean();
            }
        }
    }
}
