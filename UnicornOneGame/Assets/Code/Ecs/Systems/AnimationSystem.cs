using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Components.Refs;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class AnimationSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<AnimatorRefComponent>()
                    .Inc<MoveParametersComponent>()
                    .End();
            }

            var animatorRefPool = world.GetPool<AnimatorRefComponent>();
            var moveParametersPool = world.GetPool<MoveParametersComponent>();

            foreach (var entity in _filter)
            {
                ref var animatorRefComponent = ref animatorRefPool.Get(entity);
                ref var moveParametersComponent = ref moveParametersPool.Get(entity);

                if (animatorRefComponent.Animator.GetBool("Moving") == false)
                {
                    animatorRefComponent.Animator.SetBool("Moving", true);
                    animatorRefComponent.Animator.SetFloat("Velocity", moveParametersComponent.Speed);
                    animatorRefComponent.Animator.SetFloat("Animation Speed", 1.0f);
                }
            }
        }
    }
}
