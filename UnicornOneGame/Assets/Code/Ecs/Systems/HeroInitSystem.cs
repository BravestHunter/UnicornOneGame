using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Components.AI;
using UnicornOne.Ecs.Components.Flags;
using UnicornOne.Ecs.Components.Refs;
using UnicornOne.Ecs.Services;
using UnityEngine;
using UnityEngine.AI;

namespace UnicornOne.Ecs.Systems
{
    internal class HeroInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<HeroService> _heroService;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            SpawnHero(world); // Just one hero for now
        }

        private void SpawnHero(EcsWorld world)
        {
            var heroGameObject = GameObject.Instantiate(_heroService.Value.Prefab);
            var animator = heroGameObject.GetComponentInChildren<Animator>();
            animator.fireEvents = false;
            animator.applyRootMotion = false;
            var navigationAgent = heroGameObject.GetComponent<NavMeshAgent>();

            var entity = world.NewEntity();

            var heroFlagPool = world.GetPool<HeroFlag>();
            heroFlagPool.Add(entity);

            var meleeAtackParametersPool = world.GetPool<MeleeAtackParametersComponent>();
            ref var meleeAtackParametersComponent = ref meleeAtackParametersPool.Add(entity);
            meleeAtackParametersComponent.Damage = _heroService.Value.AttackDamage;
            meleeAtackParametersComponent.Range = _heroService.Value.AttackRange;

            var navigationPool = world.GetPool<NavigationComponent>();
            ref var navigationComponent = ref navigationPool.Add(entity);
            navigationComponent.MovementSpeed = _heroService.Value.MovingSpeed;

            var gameObjectRefPool = world.GetPool<GameObjectRefComponent>();
            ref var gameObjectRefComponent = ref gameObjectRefPool.Add(entity);
            gameObjectRefComponent.GameObject = heroGameObject;

            var animatorRefPool = world.GetPool<AnimatorRefComponent>();
            ref var animatorRefComponent = ref animatorRefPool.Add(entity);
            animatorRefComponent.Animator = animator;

            var navigationAgentRefPool = world.GetPool<NavigationAgentRefComponent>();
            ref var navigationAgentRefComponent = ref navigationAgentRefPool.Add(entity);
            navigationAgentRefComponent.Agent = navigationAgent;

            var meleeFighterBehaviorAiPool = world.GetPool<MeleeFighterBehaviorAiComponent>();
            ref var meleeFighterBehaviorAiComponent = ref meleeFighterBehaviorAiPool.Add(entity);
            meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.SearchForTarget;
        }
    }
}
