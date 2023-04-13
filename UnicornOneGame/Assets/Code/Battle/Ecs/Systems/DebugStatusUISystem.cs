using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.MonoBehaviours;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class DebugStatusUISystem : IEcsRunSystem
    {
        private const float DistanceFromCamera = 10.0f;
        private static readonly Vector3 OffsetFromObject = new Vector3(0.0f, 2.0f, 0.0f);

        private readonly EcsCustomInject<ICameraService> _cameraService;

        private readonly GameObject _debugStatusUIPrefab;

        private EcsFilter _addFilter;
        private EcsFilter _positionFilter;
        private EcsFilter _healthFilter;

        public DebugStatusUISystem(GameObject debugStatusUIPrefab)
        {
            _debugStatusUIPrefab = debugStatusUIPrefab;
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            AddDebugStatusUIs(world);
            UpdatePosition(world);
            UpdateHealthData(world);
        }

        private void AddDebugStatusUIs(EcsWorld world)
        {
            if (_addFilter == null)
            {
                _addFilter = world
                    .Filter<UnitFlag>()
                    .Inc<GameObjectUnityRefComponent>()
                    .Exc<DebugStatusUIComponent>()
                .End();
            }

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            var debugStatusUIComponentPool = world.GetPool<DebugStatusUIComponent>();
            var allyFlagPool = world.GetPool<AllyFlag>();
            var enemyFlagPool = world.GetPool<EnemyFlag>();

            foreach (var entity in _addFilter)
            {
                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);

                ref var debugStatusUIComponent = ref debugStatusUIComponentPool.Add(entity);
                debugStatusUIComponent.GameObject = GameObject.Instantiate(_debugStatusUIPrefab, gameObjectUnityRefComponent.GameObject.transform.position, Quaternion.identity);
                debugStatusUIComponent.GameObject.transform.rotation = _cameraService.Value.Camera.transform.rotation * Quaternion.Euler(0.0f, 180.0f, 0.0f);
                debugStatusUIComponent.Script = debugStatusUIComponent.GameObject.GetComponent<DebugStatusUIScript>();
                debugStatusUIComponent.TargetGameObject = gameObjectUnityRefComponent.GameObject;

                if (allyFlagPool.Has(entity))
                {
                    debugStatusUIComponent.Script.Role = Utils.Role.Ally;
                }
                else if (enemyFlagPool.Has(entity))
                {
                    debugStatusUIComponent.Script.Role = Utils.Role.Enemy;
                }
            }
        }

        private void UpdatePosition(EcsWorld world)
        {
            if (_positionFilter == null)
            {
                _positionFilter = world
                    .Filter<DebugStatusUIComponent>()
                .End();
            }

            var debugStatusUIComponentPool = world.GetPool<DebugStatusUIComponent>();

            foreach (var entity in _positionFilter)
            {
                ref var debugStatusUIComponent = ref debugStatusUIComponentPool.Get(entity);

                Vector3 upPosition = debugStatusUIComponent.TargetGameObject.transform.position + OffsetFromObject;
                Vector3 cameraPosition = _cameraService.Value.Camera.transform.position;
                Vector3 directionFromCamera = (upPosition - cameraPosition).normalized;

                debugStatusUIComponent.GameObject.transform.position = cameraPosition + DistanceFromCamera * directionFromCamera;
            }
        }

        private void UpdateHealthData(EcsWorld world)
        {
            if (_healthFilter == null)
            {
                _healthFilter = world
                    .Filter<DebugStatusUIComponent>()
                    .Inc<HealthComponent>()
                .End();
            }

            var healthComponentPool = world.GetPool<HealthComponent>();
            var debugStatusUIComponentPool = world.GetPool<DebugStatusUIComponent>();

            foreach (var entity in _healthFilter)
            {
                var healthComponent = healthComponentPool.Get(entity);
                ref var debugStatusUIComponent = ref debugStatusUIComponentPool.Get(entity);

                debugStatusUIComponent.Script.MaxHealth = healthComponent.MaxHealth;
                debugStatusUIComponent.Script.CurrentHealth = healthComponent.CurrentHealth;
            }
        }
    }
}
