using Codice.Client.BaseCommands;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.MonoBehaviours;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class HeroInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<ICameraService> _cameraService;

        private GameObject _heroPrefab;
        private GameObject _debugStatusUIPrefab;

        public HeroInitSystem(GameObject heroPrefab, GameObject debugStatusUIPrefab)
        {
            _heroPrefab = heroPrefab;
            _debugStatusUIPrefab = debugStatusUIPrefab;
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

            var debugStatusUIComponentPool = world.GetPool<DebugStatusUIComponent>();
            ref var debugStatusUIComponent = ref debugStatusUIComponentPool.Add(entity);
            debugStatusUIComponent.GameObject = Object.Instantiate(_debugStatusUIPrefab, gameObjectUnityRefComponent.GameObject.transform);
            debugStatusUIComponent.GameObject.transform.rotation = _cameraService.Value.Camera.transform.rotation * Quaternion.Euler(0.0f, 180.0f, 0.0f);
            debugStatusUIComponent.Script = debugStatusUIComponent.GameObject.GetComponent<DebugStatusUIScript>();
        }
    }
}
