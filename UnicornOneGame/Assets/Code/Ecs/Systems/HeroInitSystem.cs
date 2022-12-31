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
using UnicornOne.MonoBehaviours;
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

            foreach (HeroInfo heroInfo in _heroService.Value.Heroes)
            {
                Vector2 position = UnityEngine.Random.insideUnitCircle * 10.0f;
                SpawnHero(heroInfo, world, new Vector3(position.x, 0.0f, position.y));
            }
        }

        private void SpawnHero(HeroInfo heroInfo, EcsWorld world, Vector3 position)
        {
            var heroGameObject = GameObject.Instantiate(heroInfo.Prefab);
            heroGameObject.transform.position = position;
            var animator = heroGameObject.GetComponentInChildren<Animator>();
            animator.fireEvents = true;
            animator.applyRootMotion = false;
            var animationEventHandler = heroGameObject.GetComponentInChildren<AnimationEventHandler>();
            animationEventHandler.Clean();
            var navigationAgent = heroGameObject.GetComponent<NavMeshAgent>();

            var entity = world.NewEntity();

            var heroFlagPool = world.GetPool<HeroFlag>();
            heroFlagPool.Add(entity);

            var atackParametersPool = world.GetPool<AtackParametersComponent>();
            ref var atackParametersComponent = ref atackParametersPool.Add(entity);
            atackParametersComponent.Damage = heroInfo.AttackDamage;
            atackParametersComponent.Range = heroInfo.AttackRange;
            atackParametersComponent.AttackRechargeTime = heroInfo.AttackRechargeTime;

            var navigationPool = world.GetPool<NavigationComponent>();
            ref var navigationComponent = ref navigationPool.Add(entity);
            navigationComponent.MovementSpeed = heroInfo.MovingSpeed;

            var gameObjectRefPool = world.GetPool<GameObjectRefComponent>();
            ref var gameObjectRefComponent = ref gameObjectRefPool.Add(entity);
            gameObjectRefComponent.GameObject = heroGameObject;

            var animatorRefPool = world.GetPool<AnimatorRefComponent>();
            ref var animatorRefComponent = ref animatorRefPool.Add(entity);
            animatorRefComponent.Animator = animator;

            var animationEventHandlerRefPool = world.GetPool<AnimationEventHandlerRefComponent>();
            ref var animationEventHandlerRefComponent = ref animationEventHandlerRefPool.Add(entity);
            animationEventHandlerRefComponent.AnimationEventHandler = animationEventHandler;

            var navigationAgentRefPool = world.GetPool<NavigationAgentRefComponent>();
            ref var navigationAgentRefComponent = ref navigationAgentRefPool.Add(entity);
            navigationAgentRefComponent.Agent = navigationAgent;

            var meleeFighterBehaviorAiPool = world.GetPool<MeleeFighterBehaviorAiComponent>();
            ref var meleeFighterBehaviorAiComponent = ref meleeFighterBehaviorAiPool.Add(entity);
            meleeFighterBehaviorAiComponent.CurrentState = MeleeFighterBehaviorAiComponent.State.SearchForTarget;
        }
    }
}
