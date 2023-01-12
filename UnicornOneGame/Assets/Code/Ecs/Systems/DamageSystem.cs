using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class DamageSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<CameraService> _cameraService;
        private readonly EcsCustomInject<UIService> _uiService;

        private EcsFilter _attackFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_attackFilter == null)
            {
                _attackFilter = world
                    .Filter<DamageComponent>()
                    .Inc<TargetComponent>()
                    .End();
            }

            var damagePool = world.GetPool<DamageComponent>();
            var targetPool = world.GetPool<TargetComponent>();
            var healthPool = world.GetPool<HealthComponent>();
            var gameObjectUnityRefPool = world.GetPool<GameObjectUnityRefComponent>();

            foreach (var entity in _attackFilter)
            {
                ref var damageComponent = ref damagePool.Get(entity);
                ref var targetComponent = ref targetPool.Get(entity);

                int targetEntity;
                if (targetComponent.TargetEntity.Unpack(world, out targetEntity))
                {
                    if (healthPool.Has(targetEntity))
                    {
                        ref var healthComponent = ref healthPool.Get(targetEntity);

                        healthComponent.CurrentHealth -= damageComponent.Damage;

                        ref var gameObjectUnityRefComponent = ref gameObjectUnityRefPool.Get(targetEntity);
                        CreateDamageLabel(world, damageComponent.Damage, gameObjectUnityRefComponent.GameObject.transform.position + Vector3.up * 2.5f);
                    }
                }

                world.DelEntity(entity);
            }
        }

        private void CreateDamageLabel(EcsWorld world, int damage, Vector3 position)
        {
            var labelGameObject = GameObject.Instantiate(_uiService.Value.Label3DPrefab);
            var canvas = labelGameObject.GetComponentInChildren<Canvas>();
            var tmpText = labelGameObject.GetComponentInChildren<TMP_Text>();

            labelGameObject.transform.position = position;
            labelGameObject.transform.LookAt(_cameraService.Value.Camera.transform.position, _cameraService.Value.Camera.transform.up);
            labelGameObject.transform.Translate(Vector3.forward * 5.0f, Space.Self);
            canvas.worldCamera = _cameraService.Value.Camera;
            tmpText.text = damage.ToString();
            tmpText.fontSize = 0.5f;

            var labelEntity = world.NewEntity();

            var gameObjectUnityRefPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectUnityRefComponent = ref gameObjectUnityRefPool.Add(labelEntity);
            gameObjectUnityRefComponent.GameObject = labelGameObject;

            var canvasUnityRefPool = world.GetPool<CanvasUnityRefComponent>();
            ref var canvasUnityRefComponent = ref canvasUnityRefPool.Add(labelEntity);
            canvasUnityRefComponent.Canvas = canvas;

            var tmpTextUnityRefPool = world.GetPool<TMPTextUnityRefComponent>();
            ref var tmpTextUnityRefComponent = ref tmpTextUnityRefPool.Add(labelEntity);
            tmpTextUnityRefComponent.Text = tmpText;

            var lifetimePool = world.GetPool<LifetimeComponent>();
            ref var lifetimeComponent = ref lifetimePool.Add(labelEntity);
            lifetimeComponent.CreationTime = Time.timeSinceLevelLoad;
            lifetimeComponent.LifeDuration = 0.35f;
        }
    }
}
