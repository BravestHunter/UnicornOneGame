using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class HeroInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<ITimeService> _timeService;

        private GameObject _heroPrefab;

        public HeroInitSystem(GameObject heroPrefab)
        {
            _heroPrefab = heroPrefab;
        }

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entity = world.NewEntity();

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectUnityRefComponent = ref gameObjectUnityRefComponentPool.Add(entity);
            gameObjectUnityRefComponent.GameObject = Object.Instantiate(_heroPrefab);

            var healthComponentPool = world.GetPool<HealthComponent>();
            ref var healthComponent = ref healthComponentPool.Add(entity);
            healthComponent.MaxHealth = 100;
            healthComponent.Health = 100;
        }
    }
}
