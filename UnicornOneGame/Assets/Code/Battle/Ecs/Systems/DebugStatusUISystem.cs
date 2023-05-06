using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.MonoBehaviours;
using UnicornOne.Battle.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class DebugStatusUISystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<ICameraService> _cameraService;

        private readonly DebugStatusUISettings _settings;

        private EcsFilter _addFilter;
        private EcsFilter _updateFilter;
        private EcsFilter _healthFilter;
        private EcsFilter _aiFilter;
        private EcsFilter _abilityFilter;

        public DebugStatusUISystem(DebugStatusUISettings settings)
        {
            _settings = settings;
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            ProcessAdd(world);
            ProcessUpdate(world);
            ProcessHealthUpdate(world);
            ProcessAiUpdate(world);
            ProcessAbilityUpdate(world);
        }

        private void ProcessAdd(EcsWorld world)
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

            foreach (var entity in _addFilter)
            {
                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);

                ref var debugStatusUIComponent = ref debugStatusUIComponentPool.Add(entity);
                debugStatusUIComponent.GameObject = GameObject.Instantiate(_settings.Prefab, gameObjectUnityRefComponent.GameObject.transform.position, Quaternion.identity);
                debugStatusUIComponent.GameObject.transform.rotation = _cameraService.Value.Camera.transform.rotation * Quaternion.Euler(0.0f, 180.0f, 0.0f);
                debugStatusUIComponent.Script = debugStatusUIComponent.GameObject.GetComponent<DebugStatusUIScript>();
            }
        }

        private void ProcessUpdate(EcsWorld world)
        {
            if (_updateFilter == null)
            {
                _updateFilter = world
                    .Filter<DebugStatusUIComponent>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            var debugStatusUIComponentPool = world.GetPool<DebugStatusUIComponent>();
            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            var allyFlagPool = world.GetPool<AllyFlag>();

            foreach (var entity in _updateFilter)
            {
                ref var debugStatusUIComponent = ref debugStatusUIComponentPool.Get(entity);
                var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);

                debugStatusUIComponent.Script.UpdateSettings(_settings, allyFlagPool.Has(entity));

                // Update position
                Vector3 upPosition = gameObjectUnityRefComponent.GameObject.transform.position + _settings.OffsetFromObject;
                Vector3 cameraPosition = _cameraService.Value.Camera.transform.position;
                Vector3 directionFromCamera = (upPosition - cameraPosition).normalized;
                debugStatusUIComponent.GameObject.transform.position = cameraPosition + _settings.DistanceFromCamera * directionFromCamera;
            }
        }

        private void ProcessHealthUpdate(EcsWorld world)
        {
            if (_healthFilter == null)
            {
                _healthFilter = world
                    .Filter<DebugStatusUIComponent>()
                    .Inc<HealthComponent>()
                    .End();
            }

            var debugStatusUIComponentPool = world.GetPool<DebugStatusUIComponent>();
            var healthComponentPool = world.GetPool<HealthComponent>();

            foreach (var entity in _healthFilter)
            {
                ref var debugStatusUIComponent = ref debugStatusUIComponentPool.Get(entity);
                var healthComponent = healthComponentPool.Get(entity);

                debugStatusUIComponent.Script.HpInfo = $"HP: {healthComponent}";
            }
        }

        private void ProcessAiUpdate(EcsWorld world)
        {
            if (_aiFilter == null)
            {
                _aiFilter = world
                    .Filter<DebugStatusUIComponent>()
                    .Inc<UnitAiComponent>()
                    .End();
            }

            var debugStatusUIComponentPool = world.GetPool<DebugStatusUIComponent>();
            var unitAiComponentPool = world.GetPool<UnitAiComponent>();

            foreach (var entity in _aiFilter)
            {
                ref var debugStatusUIComponent = ref debugStatusUIComponentPool.Get(entity);
                var unitAiComponent = unitAiComponentPool.Get(entity);

                debugStatusUIComponent.Script.AiInfo = $"AI: {unitAiComponent}";
            }
        }

        private void ProcessAbilityUpdate(EcsWorld world)
        {
            if (_abilityFilter == null)
            {
                _abilityFilter = world
                    .Filter<DebugStatusUIComponent>()
                    .Inc<AbilitySetComponent>()
                    .End();
            }

            var debugStatusUIComponentPool = world.GetPool<DebugStatusUIComponent>();
            var abilityInUsageComponentPool = world.GetPool<AbilityInUsageComponent>();

            foreach (var entity in _abilityFilter)
            {
                ref var debugStatusUIComponent = ref debugStatusUIComponentPool.Get(entity);
                
                if (abilityInUsageComponentPool.Has(entity))
                {
                    var abilityInUsageComponent = abilityInUsageComponentPool.Get(entity);
                    debugStatusUIComponent.Script.AbilityInfo = $"Ability: {abilityInUsageComponent}";
                }
                else
                {
                    debugStatusUIComponent.Script.AbilityInfo = $"Ability: None";
                }
            }
        }
    }
}
