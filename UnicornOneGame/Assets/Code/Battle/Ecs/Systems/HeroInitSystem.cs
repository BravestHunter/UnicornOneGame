using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class HeroInitSystem : IEcsInitSystem
    {
        private readonly GameObject _heroPrefab;

        public HeroInitSystem(GameObject heroPrefab)
        {
            _heroPrefab = heroPrefab;
        }

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entity = world.NewEntity();

            var unitFlagPool = world.GetPool<UnitFlag>();
            unitFlagPool.Add(entity);

            var allyFlagPool = world.GetPool<AllyFlag>();
            allyFlagPool.Add(entity);

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectUnityRefComponent = ref gameObjectUnityRefComponentPool.Add(entity);
            gameObjectUnityRefComponent.GameObject = Object.Instantiate(_heroPrefab, Vector3.left * 2.5f, Quaternion.identity);

            var healthComponentPool = world.GetPool<HealthComponent>();
            ref var healthComponent = ref healthComponentPool.Add(entity);
            healthComponent.MaxHealth = 100;
            healthComponent.CurrentHealth = 100;
        }
    }
}
