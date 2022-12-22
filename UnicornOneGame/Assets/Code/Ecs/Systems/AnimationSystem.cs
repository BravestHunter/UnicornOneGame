using Leopotam.EcsLite;
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

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_meleeHeroFilter == null)
            {
                _meleeHeroFilter = world
                    .Filter<HeroFlag>()
                    .Inc<AnimatorRefComponent>()
                    .Inc<MeleeFighterBehaviorAiComponent>()
                    .Inc<NavigationComponent>()
                    .End();
            }

            var animatorRefPool = world.GetPool<AnimatorRefComponent>();
            var meleeFighterBehaviorAiPool = world.GetPool<MeleeFighterBehaviorAiComponent>();
            var navigationPool = world.GetPool<NavigationComponent>();

            var attackAnimationRequestPool = world.GetPool<AttackAnimationRequest>();

            foreach (var entity in _meleeHeroFilter)
            {
                ref var animatorRefComponent = ref animatorRefPool.Get(entity);
                ref var meleeFighterBehaviorAiComponent = ref meleeFighterBehaviorAiPool.Get(entity);
                ref var navigationComponent = ref navigationPool.Get(entity);

                animatorRefComponent.Animator.SetFloat("Animation Speed", 1.0f);

                if (meleeFighterBehaviorAiComponent.CurrentState == MeleeFighterBehaviorAiComponent.State.AttackTarget)
                {
                    animatorRefComponent.Animator.SetBool("Moving", false);
                    animatorRefComponent.Animator.SetFloat("Velocity", 0.0f);
                }
                else
                {
                    animatorRefComponent.Animator.SetBool("Moving", true);
                    animatorRefComponent.Animator.SetFloat("Velocity", navigationComponent.MovementSpeed);
                }

                if (attackAnimationRequestPool.Has(entity))
                {
                    animatorRefComponent.Animator.SetInteger("Trigger Number", 2);
                    animatorRefComponent.Animator.SetTrigger("Trigger");

                    attackAnimationRequestPool.Del(entity);
                }
            }
        }
    }
}
