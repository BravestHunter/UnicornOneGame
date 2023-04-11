using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class EnemyInitSystem : IEcsInitSystem
    {
        private readonly GameObject _enemyPrefab;

        public EnemyInitSystem(GameObject enemyPrefab)
        {
            _enemyPrefab = enemyPrefab;
        }

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entity = world.NewEntity();

            var unitFlagPool = world.GetPool<UnitFlag>();
            unitFlagPool.Add(entity);

            var enemyFlagPool = world.GetPool<EnemyFlag>();
            enemyFlagPool.Add(entity);

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectUnityRefComponent = ref gameObjectUnityRefComponentPool.Add(entity);
            gameObjectUnityRefComponent.GameObject = Object.Instantiate(_enemyPrefab, Vector3.right * 2.5f, Quaternion.identity);

            var healthComponentPool = world.GetPool<HealthComponent>();
            ref var healthComponent = ref healthComponentPool.Add(entity);
            healthComponent.MaxHealth = 200;
            healthComponent.CurrentHealth = 200;
        }
    }
}
