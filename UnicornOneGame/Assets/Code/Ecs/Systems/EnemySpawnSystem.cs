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
using UnityEngine;
using UnityEngine.AI;

namespace UnicornOne.Ecs.Systems
{
    internal class EnemySpawnSystem : IEcsRunSystem
    {
        private const int MaxEnemyCount = 10;
        private const float SpawnCirceRadius = 50.0f;

        private readonly EcsCustomInject<MobService> _mobService;
        private EcsFilter _enemyFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_enemyFilter == null)
            {
                _enemyFilter = world
                    .Filter<EnemyFlag>()
                    .End();
            }

            int enemiesToSpawn = MaxEnemyCount - _enemyFilter.GetEntitiesCount();
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // TODO: Use objects pooling

                Vector3 randomPosition = GetRandomEnemyPosition();
                SpawnEnemy(world, randomPosition);
            }
        }


        private static Vector3 GetRandomEnemyPosition()
        {
            Vector2 randomPosition = UnityEngine.Random.insideUnitCircle * SpawnCirceRadius;
            return new Vector3(randomPosition.x, 0.0f, randomPosition.y);
        }

        private void SpawnSimpleEnemy(EcsWorld world, Vector3 position)
        {
            var enemyGameObject = GameObject.Instantiate(_mobService.Value.EnemyPrefab);
            enemyGameObject.transform.position = position;
            enemyGameObject.transform.rotation = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f);

            var entity = world.NewEntity();

            var enemyFlagPool = world.GetPool<EnemyFlag>();
            enemyFlagPool.Add(entity);

            var healthPool = world.GetPool<HealthComponent>();
            ref var healthComponent = ref healthPool.Add(entity);
            healthComponent.MaxHealth = _mobService.Value.MaxHealth;
            healthComponent.CurrentHealth = healthComponent.MaxHealth;

            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectRefComponent = ref gameObjectRefPool.Add(entity);
            gameObjectRefComponent.GameObject = enemyGameObject;
        }

        private void SpawnEnemy(EcsWorld world, Vector3 position)
        {
            var gameObject = GameObject.Instantiate(_mobService.Value.EnemyPrefab);
            gameObject.transform.position = position;
            var navigationAgent = gameObject.GetComponentInChildren<NavMeshAgent>();
            var animator = gameObject.GetComponentInChildren<Animator>();
            animator.fireEvents = true;
            animator.applyRootMotion = false;
            var animationEventHandler = gameObject.GetComponentInChildren<AnimationEventHandler>();
            animationEventHandler.Clean();

            var entity = world.NewEntity();

            var enemyFlagPool = world.GetPool<EnemyFlag>();
            enemyFlagPool.Add(entity);

            var healthPool = world.GetPool<HealthComponent>();
            ref var healthComponent = ref healthPool.Add(entity);
            healthComponent.MaxHealth = _mobService.Value.MaxHealth;
            healthComponent.CurrentHealth = healthComponent.MaxHealth;

            var atackParametersPool = world.GetPool<AtackParametersComponent>();
            ref var atackParametersComponent = ref atackParametersPool.Add(entity);
            atackParametersComponent.Damage = 5;
            atackParametersComponent.Range = 1.5f;
            atackParametersComponent.AttackRechargeTime = 1.5f;

            var navigationPool = world.GetPool<NavigationComponent>();
            ref var navigationComponent = ref navigationPool.Add(entity);
            navigationComponent.MovementSpeed = 0.5f;

            var enemyBehaviorAiPool = world.GetPool<EnemyBehaviorAiComponent>();
            ref var enemyBehaviorAiComponent = ref enemyBehaviorAiPool.Add(entity);
            enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SearchForTarget;

            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectRefComponent = ref gameObjectRefPool.Add(entity);
            gameObjectRefComponent.GameObject = gameObject;

            var navigationAgentRefPool = world.GetPool<NavigationAgentUnityRefComponent>();
            ref var navigationAgentRefComponent = ref navigationAgentRefPool.Add(entity);
            navigationAgentRefComponent.Agent = navigationAgent;

            var animatorRefPool = world.GetPool<AnimatorUnityRefComponent>();
            ref var animatorRefComponent = ref animatorRefPool.Add(entity);
            animatorRefComponent.Animator = animator;

            var animationEventHandlerRefPool = world.GetPool<AnimationEventHandlerUnityRefComponent>();
            ref var animationEventHandlerRefComponent = ref animationEventHandlerRefPool.Add(entity);
            animationEventHandlerRefComponent.AnimationEventHandler = animationEventHandler;
        }
    }
}
