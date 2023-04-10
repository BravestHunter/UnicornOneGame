using Codice.CM.Client.Differences;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class DebugStatusUISystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<ICameraService> _cameraService;

        private EcsFilter _positionFilter;
        private EcsFilter _healthFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            UpdatePosition(world);
            UpdateHealthData(world);
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

                Vector3 upPosition = debugStatusUIComponent.GameObject.transform.parent.position + new Vector3(0.0f, 2.0f, 0.0f);
                Vector3 cameraPosition = _cameraService.Value.Camera.transform.position;
                Vector3 directionFromCamera = (upPosition - cameraPosition).normalized;

                debugStatusUIComponent.GameObject.transform.position = cameraPosition + 10.0f * directionFromCamera;
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

                debugStatusUIComponent.Script.SetText($"HP: {healthComponent.Health}/{healthComponent.MaxHealth}");
            }
        }
    }
}
