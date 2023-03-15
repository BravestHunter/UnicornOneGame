using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Services;
using UnicornOne.MonoBehaviours;
using UnicornOne.ScriptableObjects.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace UnicornOne.Ecs.Systems
{
    internal class HeroInitSystem : IEcsInitSystem
    {
        private const float HeroSpawnRange = 5.0f;

        private readonly EcsCustomInject<HeroService> _heroService;
        private readonly EcsCustomInject<AbilityService> _abilityService;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (IHero hero in _heroService.Value.Heroes)
            {
                Vector2 position = UnityEngine.Random.insideUnitCircle * HeroSpawnRange;

                SpawnHero(hero, world, new Vector3(position.x, 0.0f, position.y));
            }
        }

        private void SpawnHero(IHero hero, EcsWorld world, Vector3 position)
        {
            var heroGameObject = GameObject.Instantiate(hero.PrefabInfo.Prefab);
            heroGameObject.transform.position = position;
            var animator = heroGameObject.GetComponentInChildren<Animator>();
            animator.fireEvents = true;
            animator.applyRootMotion = false;
            var animationEventHandler = heroGameObject.GetComponentInChildren<AnimationEventHandler>();
            animationEventHandler.Clean();
            var navigationAgent = heroGameObject.GetComponent<NavMeshAgent>();
            var launchPoint = heroGameObject.GetComponentInChildren<LaunchPoint>();
            var targetPoint = heroGameObject.GetComponentInChildren<TargetPoint>();

            var entity = world.NewEntity();

            var heroFlagPool = world.GetPool<HeroFlag>();
            heroFlagPool.Add(entity);

            var healthPool = world.GetPool<HealthComponent>();
            ref var healthComponent = ref healthPool.Add(entity);
            healthComponent.MaxHealth = hero.HealthInfo.Health;
            healthComponent.CurrentHealth = healthComponent.MaxHealth;

            var navigationPool = world.GetPool<NavigationComponent>();
            ref var navigationComponent = ref navigationPool.Add(entity);
            navigationComponent.MovementSpeed = hero.MoveInfo.Speed;

            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectRefComponent = ref gameObjectRefPool.Add(entity);
            gameObjectRefComponent.GameObject = heroGameObject;

            var animatorRefPool = world.GetPool<AnimatorUnityRefComponent>();
            ref var animatorRefComponent = ref animatorRefPool.Add(entity);
            animatorRefComponent.Animator = animator;

            var animationEventHandlerRefPool = world.GetPool<AnimationEventHandlerUnityRefComponent>();
            ref var animationEventHandlerRefComponent = ref animationEventHandlerRefPool.Add(entity);
            animationEventHandlerRefComponent.AnimationEventHandler = animationEventHandler;

            var navigationAgentRefPool = world.GetPool<NavigationAgentUnityRefComponent>();
            ref var navigationAgentRefComponent = ref navigationAgentRefPool.Add(entity);
            navigationAgentRefComponent.Agent = navigationAgent;

            if (launchPoint != null)
            {
                var launchPointRefPool = world.GetPool<LaunchPointUnityRefComponent>();
                ref var launchPointUnityRefComponent = ref launchPointRefPool.Add(entity);
                launchPointUnityRefComponent.LaunchPoint = launchPoint;
            }

            if (targetPoint != null)
            {
                var targetPointRefPool = world.GetPool<TargetPointUnityRefComponent>();
                ref var targetPointUnityRefComponent = ref targetPointRefPool.Add(entity);
                targetPointUnityRefComponent.TargetPoint = targetPoint;
            }

            var heroBehaviorAiPool = world.GetPool<HeroBehaviorAiComponent>();
            ref var heroBehaviorAiComponent = ref heroBehaviorAiPool.Add(entity);
            heroBehaviorAiComponent.CurrentState = HeroBehaviorAiComponent.State.SearchingForTarget;

            var abilitySetPool = world.GetPool<AbilitySetComponent>();
            ref var abilitySetComponent = ref abilitySetPool.Add(entity);
            abilitySetComponent.Index = _abilityService.Value.HeroToAbilitySetMap[hero];

            var abilityRechargePool = world.GetPool<AbilityRechargeComponent>();
            ref var abilityRechargeComponent = ref abilityRechargePool.Add(entity);
            abilityRechargeComponent.LastUseTimes = Enumerable.Repeat(float.NegativeInfinity, 4).ToArray();
        }
    }
}
