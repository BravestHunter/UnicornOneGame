﻿using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Components.AI;
using UnicornOne.Ecs.Components.Flags;
using UnicornOne.Ecs.Components.Refs;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class AnimationSystem : IEcsRunSystem
    {
        private EcsFilter _meleeHeroFilter;
        private EcsFilter _animationEventFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessMeleeHeroAnimation(world);
            ProcessAnimationEvents(world);
        }

        private void ProcessMeleeHeroAnimation(EcsWorld world)
        {
            if (_meleeHeroFilter == null)
            {
                _meleeHeroFilter = world
                    .Filter<HeroFlag>()
                    .Inc<AnimatorRefComponent>()
                    .Inc<AnimationEventHandlerRefComponent>()
                    .Inc<MeleeFighterBehaviorAiComponent>()
                    .Inc<NavigationComponent>()
                    .End();
            }

            var animatorRefPool = world.GetPool<AnimatorRefComponent>();
            var navigationPool = world.GetPool<NavigationComponent>();
            var attackAnimationRequestPool = world.GetPool<AttackAnimationRequest>();
            var attackAnimationFlagPool = world.GetPool<AttackAnimationFlag>();

            foreach (var entity in _meleeHeroFilter)
            {
                ref var animatorRefComponent = ref animatorRefPool.Get(entity);
                ref var navigationComponent = ref navigationPool.Get(entity);

                animatorRefComponent.Animator.SetFloat("Animation Speed", 1.0f); // Always 1.0 by now

                if (attackAnimationRequestPool.Has(entity))
                {
                    animatorRefComponent.Animator.SetBool("Moving", false);
                    animatorRefComponent.Animator.SetFloat("Velocity", 0.0f);

                    animatorRefComponent.Animator.SetInteger("Trigger Number", 2);
                    animatorRefComponent.Animator.SetTrigger("Trigger");

                    attackAnimationFlagPool.Add(entity);
                    attackAnimationRequestPool.Del(entity);

                    continue;
                }

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

                animatorRefComponent.Animator.SetBool("Moving", true);
                animatorRefComponent.Animator.SetFloat("Velocity", navigationComponent.MovementSpeed);
            }
        }

        private void ProcessAnimationEvents(EcsWorld world)
        {
            if (_animationEventFilter == null)
            {
                _animationEventFilter = world
                    .Filter<AnimationEventHandlerRefComponent>()
                    .End();
            }

            var animationEventHandlerRefPool = world.GetPool<AnimationEventHandlerRefComponent>();
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
