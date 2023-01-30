﻿using Leopotam.EcsLite;
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

        private readonly EcsCustomInject<LevelService> _levelService;
        private readonly EcsCustomInject<MobService> _mobService;

        private EcsFilter _enemyFilter;

        private int _waveCounter = 0;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_waveCounter >= _levelService.Value.Level.Script.Waves.Length)
            {
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
            Vector3 spawnPoint = _levelService.Value.Level.EnemySpawnPositions[UnityEngine.Random.Range(0, _levelService.Value.Level.EnemySpawnPositions.Length - 1)];

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

        private void SpawnSimpleEnemy(EcsWorld world, Vector3 position)
        {
            var enemyGameObject = GameObject.Instantiate(_mobService.Value.Enemy.PrefabInfo.Prefab);
            enemyGameObject.transform.position = position;
            enemyGameObject.transform.rotation = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f);

            var entity = world.NewEntity();

            var enemyFlagPool = world.GetPool<EnemyFlag>();
            enemyFlagPool.Add(entity);

            var healthPool = world.GetPool<HealthComponent>();
            ref var healthComponent = ref healthPool.Add(entity);
            healthComponent.MaxHealth = _mobService.Value.Enemy.HealthInfo.Health;
            healthComponent.CurrentHealth = healthComponent.MaxHealth;

            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectRefComponent = ref gameObjectRefPool.Add(entity);
            gameObjectRefComponent.GameObject = enemyGameObject;
        }

        private static void SpawnEnemy(Enemy enemy, Vector3 position, EcsWorld world)
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

            var atackParametersPool = world.GetPool<AtackParametersComponent>();
            ref var atackParametersComponent = ref atackParametersPool.Add(entity);
            atackParametersComponent.Damage = enemy.AttackInfo.Damage;
            atackParametersComponent.Range = enemy.AttackInfo.Range;
            atackParametersComponent.AttackRechargeTime = enemy.AttackInfo.RechargeTime;

            var navigationPool = world.GetPool<NavigationComponent>();
            ref var navigationComponent = ref navigationPool.Add(entity);
            navigationComponent.MovementSpeed = enemy.MoveInfo.Speed;

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
