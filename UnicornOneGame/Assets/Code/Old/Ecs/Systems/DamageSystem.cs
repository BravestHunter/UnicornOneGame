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
using UnityEditor;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class DamageSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<GameSettingsService> _gameSettingsService;
        private readonly EcsCustomInject<CameraService> _cameraService;

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
            var enemyFlagPool = world.GetPool<EnemyFlag>();
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
                    }

                    // Only enemies spawn damage labels
                    if (enemyFlagPool.Has(targetEntity) && gameObjectUnityRefPool.Has(targetEntity))
                    {
                        ref var gameObjectUnityRefComponent = ref gameObjectUnityRefPool.Get(targetEntity);
                        CreateDamageLabel(world, damageComponent.Damage, gameObjectUnityRefComponent.GameObject.transform.position + Vector3.up * 2.5f);
                    }
                }

                world.DelEntity(entity);
            }
        }

        private void CreateDamageLabel(EcsWorld world, int damage, Vector3 position)
        {
            var labelGameObject = GameObject.Instantiate(_gameSettingsService.Value.DamageNumbers.Prefab);
            var canvas = labelGameObject.GetComponentInChildren<Canvas>();
            var tmpText = labelGameObject.GetComponentInChildren<TMP_Text>();
            var animator = labelGameObject.GetComponentInChildren<Animator>();

            labelGameObject.transform.position = position;
            labelGameObject.transform.LookAt(_cameraService.Value.Camera.transform.position, _cameraService.Value.Camera.transform.up);
            labelGameObject.transform.Translate(_gameSettingsService.Value.DamageNumbers.SpawnPointOffset, Space.Self);
            canvas.worldCamera = _cameraService.Value.Camera;
            tmpText.text = damage.ToString();
            tmpText.fontSize = _gameSettingsService.Value.DamageNumbers.FontSize;
            tmpText.color = _gameSettingsService.Value.DamageNumbers.Color;
            tmpText.font = _gameSettingsService.Value.DamageNumbers.Font;
            animator.SetFloat("AnimationSpeed", 1.0f / _gameSettingsService.Value.DamageNumbers.Lifetime);

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

            var animatorUnityRefComponentPool = world.GetPool<AnimatorUnityRefComponent>();
            ref var animatorUnityRefComponent = ref animatorUnityRefComponentPool.Add(labelEntity);
            animatorUnityRefComponent.Animator = animator;

            var lifetimePool = world.GetPool<LifetimeComponent>();
            ref var lifetimeComponent = ref lifetimePool.Add(labelEntity);
            lifetimeComponent.CreationTime = Time.timeSinceLevelLoad;
            lifetimeComponent.LifeDuration = _gameSettingsService.Value.DamageNumbers.Lifetime;
        }
    }
}
