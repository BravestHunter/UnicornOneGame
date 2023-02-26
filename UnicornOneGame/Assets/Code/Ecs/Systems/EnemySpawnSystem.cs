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
using UnicornOne.ScriptableObjects;
using UnicornOne.ScriptableObjects.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace UnicornOne.Ecs.Systems
{
    internal class EnemySpawnSystem : IEcsRunSystem
    {
        private const float SpawnCirceRadius = 6.0f;

        private readonly EcsCustomInject<GameControlService> _gameControlService;
        private readonly EcsCustomInject<LevelService> _levelService;
        private readonly EcsCustomInject<AbilityService> _abilityService;

        private EcsFilter _enemyFilter;

        private int _waveCounter = 0;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_waveCounter >= _levelService.Value.Level.Script.Waves.Length)
            {
                if (!ShouldSpawnNextWave(world))
                {
                    _gameControlService.Value.ReportGameFinish();
                }    

                return;
            }

            if (!ShouldSpawnNextWave(world))
            {
                return;
            }

            SpawnEnemyWave(world);
        }

        private bool ShouldSpawnNextWave(EcsWorld world)
        {
            if (_enemyFilter == null)
            {
                _enemyFilter = world
                    .Filter<EnemyFlag>()
                    .End();
            }

            return _enemyFilter.GetEntitiesCount() == 0;
        }

        private void SpawnEnemyWave(EcsWorld world)
        {
            var wave = _levelService.Value.Level.Script.Waves[_waveCounter];
            Vector3 spawnPoint = _levelService.Value.SpawnPoints[_waveCounter];

            for (int i = 0; i < wave.Count; i++)
            {
                Vector3 randomPosition = GetRandomPositionInRadius() + spawnPoint;

                // TODO: Use objects pooling
                SpawnEnemy(wave.Enemy, randomPosition, world);
            }

            _waveCounter++;
        }

        private static Vector3 GetRandomPositionInRadius()
        {
            Vector2 randomPosition = UnityEngine.Random.insideUnitCircle * SpawnCirceRadius;
            return new Vector3(randomPosition.x, 0.0f, randomPosition.y);
        }

        private void SpawnEnemy(IEnemy enemy, Vector3 position, EcsWorld world)
        {
            var gameObject = GameObject.Instantiate(enemy.PrefabInfo.Prefab);
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
            healthComponent.MaxHealth = enemy.HealthInfo.Health;
            healthComponent.CurrentHealth = healthComponent.MaxHealth;

            var navigationPool = world.GetPool<NavigationComponent>();
            ref var navigationComponent = ref navigationPool.Add(entity);
            navigationComponent.MovementSpeed = enemy.MoveInfo.Speed;

            var enemyBehaviorAiPool = world.GetPool<EnemyBehaviorAiComponent>();
            ref var enemyBehaviorAiComponent = ref enemyBehaviorAiPool.Add(entity);
            enemyBehaviorAiComponent.CurrentState = EnemyBehaviorAiComponent.State.SearchingForTarget;

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

            var abilitySetPool = world.GetPool<AbilitySetComponent>();
            ref var abilitySetComponent = ref abilitySetPool.Add(entity);
            abilitySetComponent.Index = _abilityService.Value.EnemyToAbilitySetMap[enemy];

            var abilityRechargePool = world.GetPool<AbilityRechargeComponent>();
            ref var abilityRechargeComponent = ref abilityRechargePool.Add(entity);
            abilityRechargeComponent.LastUseTimes = Enumerable.Repeat(float.NegativeInfinity, 4).ToArray();
        }
    }
}
